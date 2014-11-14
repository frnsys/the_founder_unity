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
 * - Similarly, ScriptableObjects must have any setup manually run.
 *
 *
 * TO DO:
 * =========================
 * - this does not serialize null values, but that may be useful.
 */

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Serializer {

    [Serializable]
    public struct Serialized {
        public Dictionary<string, object> tree;

        // Although typeRef is never used for deserialization,
        // it's nice to have on hand in case something changes.
        // Then we will at least remember what the original types were.
        public Dictionary<string, Type> typeRef;

        public Serialized(Dictionary<string, object> t, Dictionary<string, Type> tr) {
            tree = t;
            typeRef = tr;
        }
    }




    // ===============================================
    // Serialization =================================
    // ===============================================

    public static Serialized Serialize(object obj) {
        Type type = obj.GetType();

        // This tree maps the object hierarchy.
        Dictionary<string, object> tree = new Dictionary<string, object>();
        Dictionary<string, Type> typeRef = new Dictionary<string, Type>();

        // Each public field which is not specifically marked "nonserialized".
        // Each private field which is specifically marked "serialized".
        foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            // Check if the field is something we're allowed to serialize.
            if (IsSerializable(fi)) {

                Type ft = fi.FieldType;
                string name = fi.Name;
                object val = fi.GetValue(obj);

                if (IsSimpleType(ft)) {
                    tree.Add(name, val);
                    typeRef.Add(name, ft);

                // Check if it is a property with a generic type.
                } else if (ft.IsGenericType) {
                    Type g = ft.GetGenericArguments()[0];

                    // If it a simple generic type, just save it.
                    if (IsSimpleType(g)) {
                        tree.Add(name, val);
                        typeRef.Add(name, ft);

                    // Otherwise, handle the generics if they are serializable.
                    } else if (g.IsSerializable) {

                        // This only supports lists at the moment.
                        //if (ft.GetGenericTypeDefinition() == typeof(List<>)) {
                        if (val is IEnumerable) {
                            List<Serialized> ser = new List<Serialized>();
                            foreach (object o in (IEnumerable)val) {
                                ser.Add(Serialize(o));
                            }

                            tree.Add(name, ser);
                            typeRef.Add(name, ft);
                        }

                    }

                // Handle other classes marked for serialization.
                } else if (ft.IsSerializable && val != null) {
                    tree.Add(name, Serialize(val));
                    typeRef.Add(name, ft);

                } else {
                    Debug.Log("This serializable field was not serialized. Maybe you forgot to mark it as [Serializable]?");
                    Debug.Log(fi.Name + " :: " + fi.FieldType.ToString());
                }
            }
        }

        // If it is a ScriptableObject, get its name as well.
        if (IsScriptableObject(type)) {
            tree.Add("name", type.GetProperty("name").GetValue(obj, null));
            typeRef.Add("name", typeof(string));
        }

        return new Serialized(tree, typeRef);
    }





    // ===============================================
    // Deserialization ===============================
    // ===============================================

    public static T Deserialize<T>(Serialized serialized) where T : ScriptableObject {
        T obj = ScriptableObject.CreateInstance<T>();
        return (T)Deserialize(serialized, obj);
    }
    private static T DeserializeClass<T>(Serialized serialized) {
        T obj = (T)Activator.CreateInstance(typeof(T));
        return (T)Deserialize(serialized, obj);
    }
    private static object Deserialize(Serialized serialized, object obj) {
        Type type = obj.GetType();
        foreach (FieldInfo fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            if (IsSerializable(fi) && serialized.tree.Keys.Contains(fi.Name)) {
                Type ft = fi.FieldType;
                string name = fi.Name;
                object val = serialized.tree[name];

                if (IsSimpleType(ft)) {
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
                            foreach (Serialized o in (List<Serialized>)val) {
                                l.Add(Deserialize(o, g));
                            }
                            fi.SetValue(obj, Convert.ChangeType(l, ft));
                        }

                    }

                // Handle other classes marked for serialization.
                } else if (ft.IsSerializable && val != null) {
                    object o = Deserialize((Serialized)val, ft);
                    fi.SetValue(obj, Convert.ChangeType(o, ft));
                }
            }
        }

        // If it is a ScriptableObject, get its name as well.
        if (IsScriptableObject(type)) {
            PropertyInfo prop = type.GetProperty("name");
            prop.SetValue(obj, (string)serialized.tree["name"], null);
        }

        return obj;
    }

    private static object Deserialize(Serialized serialized, Type t) {
        // I don't like this anymore than you do
        MethodInfo method;
        if (IsScriptableObject(t)) {
            method = typeof(Serializer).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
        } else {
            method = typeof(Serializer).GetMethod("DeserializeClass", BindingFlags.NonPublic | BindingFlags.Static);
        }

        method = method.MakeGenericMethod(t);
        return method.Invoke(null, new object[] {serialized});
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
}
