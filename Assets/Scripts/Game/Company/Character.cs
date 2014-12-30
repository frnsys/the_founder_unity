using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Character : ScriptableObject {
    public enum Type {
        Journalist,
        Investor
    }

    public static List<Character> LoadAll() {
        // Load workers as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<Character>("Characters").ToList().Select(w => Instantiate(w) as Character).ToList();
    }

    public Type type;
    public Stat relationship;

    public void Init(string name_, Type type_) {
        name = name_;
        type = type_;
        relationship = new Stat("Relationship", 0);
    }
}
