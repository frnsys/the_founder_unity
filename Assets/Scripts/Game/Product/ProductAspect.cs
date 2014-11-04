using UnityEngine;
using System.Collections;

public abstract class ProductAspect : ScriptableObject, IUnlockable {
    public string description;

    // Product points.
    public int points;

    public override string ToString() {
        return name;
    }
}
