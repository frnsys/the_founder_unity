using UnityEngine;
using System.Collections;

public abstract class ProductAspect : ScriptableObject, IUnlockable {
    public string description;

    public override string ToString() {
        return name;
    }

    #region IUnlockable implementation
    private bool unlocked = false;
    public bool Unlocked {
        get {
            return unlocked;
        }
        set {
            unlocked = value;
        }
    }
    #endregion
}
