using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A group of stats.
[System.Serializable]
public class Stat {

    private string _name;
    public string name {
        get { return _name; }
    }
    public float baseValue = 0;
    public StatBuffCollection buffs = new StatBuffCollection();

    public float value {
        get {
            float finalValue = baseValue;
            foreach (StatBuff buff in buffs) {
                if (buff.type == BuffType.ADD) {
                    finalValue += buff.value;
                } else {
                    finalValue *= buff.value;
                }
            }
            return finalValue;
        }
    }

    public Stat(string name_, float baseValue_ = 0) {
        _name = name_;
        baseValue = baseValue_;
    }
}
