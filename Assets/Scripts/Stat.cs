using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A group of stats.
public class Stat {
    public string name;
    public float baseValue = 0;
    public StatModifierCollection modifiers = new StatModifierCollection();

    public float finalValue {
        get {
            float _finalValue = baseValue;
            foreach (StatModifier mod in modifiers) {
                if (mod.type == ModifierType.add) {
                    _finalValue += mod.value;
                } else {
                    _finalValue *= mod.value;
                }
            }
            return _finalValue;
        }
    }

    public Stat(string name_, float baseValue_ = 0) {
        name = name_;
        baseValue = baseValue_;
    }
}
