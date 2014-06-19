using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A group of stats.
public class Stat {
    public string name;
    public float baseValue = 0;
    public StatBuffCollection buffs = new StatBuffCollection();

    public float value {
        get {
            float finalValue = baseValue;
            foreach (StatBuff buff in buffs) {
                if (buff.type == BuffType.add) {
                    finalValue += buff.value;
                } else {
                    finalValue *= buff.value;
                }
            }
            return finalValue;
        }
    }

    public Stat(string name_, float baseValue_ = 0) {
        name = name_;
        baseValue = baseValue_;
    }
}
