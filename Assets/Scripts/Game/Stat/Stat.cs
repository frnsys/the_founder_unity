/*
 * A Stat is some number value which can be modified by buffs.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Stat {

    private string _name;
    public string name {
        get { return _name; }
    }
    public float value {
        get { return baseValue + acc; }
    }
    public float baseValue = 0;
    public float acc = 0;

    [SerializeField]
    private List<StatBuff> _buffs = new List<StatBuff>();

    public void ApplyBuff(StatBuff buff) {
        _buffs.Add(buff);
        acc += buff.value;
    }

    public void RemoveBuff(StatBuff buff) {
        if (_buffs.Remove(buff)) {
            acc -= buff.value;
        }
    }

    public Stat(string name_, float baseValue_ = 0) {
        _name = name_;
        baseValue = baseValue_;
    }
    public Stat() {
        _name = "DEFAULTNAME";
        baseValue = 0;
    }

    public override string ToString() {
        return value.ToString();
    }
}
