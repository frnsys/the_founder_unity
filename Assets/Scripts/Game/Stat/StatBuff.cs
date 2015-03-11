/*
 * A Buff modifies a Stat.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StatBuff {
    public string name;
    public float value;

    public StatBuff(string name_, float value_) {
        name = name_;
        value = value_;
    }
    public StatBuff() {
        name = "DEFAULTNAME";
        value = 0;
    }

    public bool Equals(StatBuff sb) {
        return name == sb.name && value == sb.value;
    }
}
