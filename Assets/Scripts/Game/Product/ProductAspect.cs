using UnityEngine;
using System.Collections;

public abstract class ProductAspect : ScriptableObject, IUnlockable {
    public string description;

    public override string ToString() {
        return name;
    }
}
