/*
 * This class can properly serialize (and serialize) ScriptableObjects
 * so we can save them to a binary file.
 *
 * NOTES:
 * =========================
 * - Add the [SerializeField] attribute to any private fields you want serialized.
 * - Public fields are serialized by default. Add the [NonSerialized] attribute to skip them.
 * - Any class, including ScriptableObject subclasses, which are to be serialized must have the [Serializable] attribute.
 * - Any non-ScriptableObject class which is to be serialized MUST have a parameterless constructor. Keep this in mind if you need to do some initialization stuff.
 * - Similarly, ScriptableObjects must have any setup/initialization manually run.
 *
 *
 * TO DO:
 * =========================
 * - this does not serialize null values, but that may be useful.
 */

using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Serializer {

    [Serializable]
    // This is a representation of a SerializedObject or a non-SerializedObject class
    // in a form that can be serialized.
    public class Replica {
        // Matches field names to data.
        public Dictionary<string, object> map;

        // Although typeRef is never used for deserialization,
        // it's nice to have on hand in case something changes.
        // Then we will at least remember what the original types were.
        public Dictionary<string, Type> typeRef;

        public Replica() {
            map = new Dictionary<string, object>();
            typeRef = new Dictionary<string, Type>();
        }

        public void Add(string name, object obj, Type type) {
            map.Add(name, obj);
            typeRef.Add(name, type);
        }
    }




    // ===============================================
    // Serialization =================================
    // ===============================================

    // Serialize an object.
    public static Replica Serialize(object obj) {
        Replica replica = new Replica();
        Type type = obj.GetType();

        // If this object is a resource, we need only serialize its name,
        // and skip everything else.
        if (IsResource(type)) {
            replica.Add("name", type.GetProperty("name").GetValue(obj, null), typeof(string));
            return replica;
        }

        // For...
        // each public field which is not specifically marked "nonserialized".
        // each private field which is specifically marked "serialized".
        foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            // Check if the field is something we're allowed to serialize.
            if (IsSerializable(fi)) {

                Type ft = fi.FieldType;
                string name = fi.Name;
                object val = fi.GetValue(obj);

                // If this field references a resource,
                // we don't want to serialize the entire resource.
                // Just enough data (name and type) so that we can reload it on deserialization.
                if (IsResource(ft)) {
                    ScriptableObject resource = (ScriptableObject)val;
                    replica.Add(name, resource.name, ft);

                // If it is a simple type, we can just serialize it as-is.
                } else if (IsSimpleType(ft)) {
                    replica.Add(name, val, ft);

                // Check if it is a property with a generic type.
                } else if (ft.IsGenericType) {
                    Type g = ft.GetGenericArguments()[0];

                    // If it a simple type, just save it.
                    if (IsSimpleType(g)) {
                        replica.Add(name, val, ft);

                    // Otherwise, handle the generics if they are serializable.
                    } else if (g.IsSerializable) {

                        // This only supports lists at the moment.
                        if (val is IEnumerable) {
                            List<Replica> sl = new List<Replica>();
                            foreach (object o in (IEnumerable)val) {
                                sl.Add(Serialize(o));
                            }
                            replica.Add(name, sl, ft);
                        }

                    }

                // Handle other classes (non-simple, non-generic, non-resource) marked for serialization.
                } else if (ft.IsSerializable && val != null) {
                    replica.Add(name, Serialize(val), ft);

                } else {
                    Debug.Log("This serializable field was not serialized. Maybe you forgot to mark it as [Serializable]?");
                    Debug.Log("\t" + fi.Name + " :: " + fi.FieldType.ToString());
                }
            }
        }

        // If it is a ScriptableObject, get its name as well.
        if (IsScriptableObject(type)) {
            replica.Add("name", type.GetProperty("name").GetValue(obj, null), typeof(string));
        }

        return replica;
    }





    // ===============================================
    // Deserialization ===============================
    // ===============================================

    public static T Deserialize<T>(Replica replica) where T : ScriptableObject {
        T obj = ScriptableObject.CreateInstance<T>();
        return (T)Deserialize(replica, obj);
    }

    // Deserializes a non-ScriptableObject class instance.
    private static T DeserializeClass<T>(Replica replica) {
        T obj = (T)Activator.CreateInstance(typeof(T));
        return (T)Deserialize(replica, obj);
    }

    private static object Deserialize(Replica replica, Type t) {
        // I don't like this anymore than you do
        MethodInfo method;
        if (IsScriptableObject(t)) {
            method = typeof(Serializer).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
        } else {
            method = typeof(Serializer).GetMethod("DeserializeClass", BindingFlags.NonPublic | BindingFlags.Static);
        }

        method = method.MakeGenericMethod(t);
        return method.Invoke(null, new object[] {replica});
    }

    private static object Deserialize(Replica replica, object obj) {
        Type type = obj.GetType();

        // If this object is a resource, we just reload it.
        if (IsResource(type)) {
            string resourceName = (string)replica.map["name"];
            return type.GetMethod("Load").Invoke(null, new object[] { resourceName });
        }

        foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            if (IsSerializable(fi) && replica.map.Keys.Contains(fi.Name)) {
                Type ft = fi.FieldType;
                string name = fi.Name;
                object val = replica.map[name];

                // If this field references a resource,
                // reload it based on the name and type.
                if (IsResource(ft)) {
                    string resourceName = (string)val;
                    fi.SetValue(obj, ft.GetMethod("Load").Invoke(null, new object[] { resourceName }));

                } else if (IsSimpleType(ft)) {
                    fi.SetValue(obj, Convert.ChangeType(val, ft));

                // Check if it is a property with a generic type.
                } else if (ft.IsGenericType) {
                    Type g = ft.GetGenericArguments()[0];

                    // If it a simple generic type, just load it.
                    if (IsSimpleType(g)) {
                        fi.SetValue(obj, Convert.ChangeType(val, ft));

                    // Otherwise, handle the generics if they are serializable.
                    } else if (g.IsSerializable) {

                        // This only supports lists at the moment.
                        if (val is IEnumerable) {
                            Type genericListType = typeof(List<>).MakeGenericType(g);
                            IList l = (IList)Activator.CreateInstance(genericListType);
                            foreach (Replica r in (List<Replica>)val) {
                                l.Add(Deserialize(r, g));
                            }
                            fi.SetValue(obj, Convert.ChangeType(l, ft));
                        }

                    }

                // Handle other classes marked for serialization.
                } else if (ft.IsSerializable && val != null) {
                    object o = Deserialize((Replica)val, ft);
                    fi.SetValue(obj, Convert.ChangeType(o, ft));
                }
            }
        }

        // If it is a ScriptableObject, get its name as well.
        if (IsScriptableObject(type)) {
            PropertyInfo prop = type.GetProperty("name");
            prop.SetValue(obj, (string)replica.map["name"], null);
        }

        return obj;
    }







    // ===============================================
    // Utility =======================================
    // ===============================================

    private static bool IsScriptableObject(Type type) {
        return type.IsSubclassOf(typeof(ScriptableObject));
    }

    private static bool IsSerializable(FieldInfo fi) {
        return (fi.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0 && fi.IsPublic) ||
               (fi.GetCustomAttributes(typeof(SerializeField), true).Length > 0 && fi.IsPrivate);
    }

    private static bool IsResource(Type type) {
        return IsSubclassOfRawGeneric(typeof(Resource<>), type);
    }

    // https://gist.github.com/jonathanconway/3330614
    // https://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive
    private static bool IsSimpleType(Type type) {
		return
			type.IsValueType ||
			type.IsPrimitive ||
			new Type[] {
				typeof(String),
				typeof(Decimal),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(Guid)
			}.Contains(type) ||
			Convert.GetTypeCode(type) != TypeCode.Object;
    }

    // http://stackoverflow.com/a/457708/1097920
    static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
        while (toCheck != null && toCheck != typeof(object)) {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    // NOT USED ATM
    // Use this custom attribute to tag classes which are serializable, but should be treated as Resources.
    //[System.AttributeUsage(System.AttributeTargets.Class)]
    //public class Resource : System.Attribute {}
}

// Resources are not serialized/deserialized,
// the resource name is remembered and then they are just reloaded.
public class Resource<T> : ScriptableObject where T : UnityEngine.Object {
    public static T Load(string name) {
        return Resources.Load<T>(name);
    }
}
