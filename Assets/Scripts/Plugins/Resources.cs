using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// SharedResources are resources/assets which are never modified and should only have
// one instance in play.
// They are not serialized/deserialized; since they are never modified,
// the resource name is remembered and then they are just reloaded from scratch.
// This ensures that only one instance is created.
public class SharedResource<T> : ScriptableObject where T : UnityEngine.Object {
    public static T Load(string name) {
        return Resources.Load<T>(name);
    }
}

public class TemplateResource<T> : ScriptableObject where T : UnityEngine.Object {
    // Create a new instance which can be safely modified.
    public T Clone() {
        T res = Instantiate(this) as T;
        res.name = name;
        return res;
    }

    // Find a matching instance for the given instance.
    public static T Find(T res, IEnumerable<T> reses) {
        return reses.Where(r => r.name == res.name).SingleOrDefault();
    }
}
