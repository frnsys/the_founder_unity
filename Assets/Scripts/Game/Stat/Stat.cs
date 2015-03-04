/*
 * A Stat is some number value which can be modified by buffs.
 */

using UnityEngine;
using System.Timers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Serializable]
public class Stat {

    private string _name;
    public string name {
        get { return _name; }
    }
    public float baseValue = 0;

    [SerializeField]
    private List<StatBuff> _buffs = new List<StatBuff>();
    public ReadOnlyCollection<StatBuff> buffs {
        get { return _buffs.AsReadOnly(); }
    }

    public float value {
        get {
            float finalValue = baseValue;
            foreach (StatBuff buff in _buffs) {
                finalValue += buff.value;
            }
            return finalValue;
        }
    }

    public void ApplyBuff(StatBuff buff) {
        _buffs.Insert(0, buff);
    }

    public void RemoveBuff(StatBuff buff) {
        _buffs.Remove(buff);
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
