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
