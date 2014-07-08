using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;

[System.Serializable]
public class Stat {

    private string _name;
    public string name {
        get { return _name; }
    }
    public float baseValue = 0;

    private List<StatBuff> _buffs = new List<StatBuff>();
    public ReadOnlyCollection<StatBuff> buffs {
        get { return _buffs.AsReadOnly(); }
    }

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

    public void ApplyBuff(StatBuff buff) {
        // Buffs that add are calculated first,
        // so they are inserted at the beginning.
        if (buff.type == BuffType.ADD) {
            _buffs.Insert(0, buff);

        // Multiplier buffs are calculated at the end.
        } else {
            _buffs.Insert(_buffs.Count, buff);
        }

        if (buff.duration > 0) {
            Timer timer = new Timer(buff.duration);
            timer.Elapsed += delegate { _buffs.Remove(buff); };
            timer.Start();
        }
    }

    public void RemoveBuff(StatBuff buff) {
        _buffs.Remove(buff);
    }

    public Stat(string name_, float baseValue_ = 0) {
        _name = name_;
        baseValue = baseValue_;
    }

    public override string ToString() {
        return value.ToString();
    }
}
