using UnityEngine;

[System.Serializable]
public class Condition : ScriptableObject {
    public virtual bool Evaluate(Company company) {
        return false;
    }
}
