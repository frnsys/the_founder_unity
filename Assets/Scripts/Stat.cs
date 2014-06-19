using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A group of stats.
public class Stat {
    public string name;
    public float baseValue = 0;
    public StatBuffCollection buffs = new StatBuffCollection();

    public float finalValue {
        get {
            float _finalValue = baseValue;
            foreach (StatBuff buff in buffs) {
                if (buff.type == BuffType.add) {
                    _finalValue += buff.value;
                } else {
                    _finalValue *= buff.value;
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
