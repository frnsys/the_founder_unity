using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public enum BuffType {ADD, MULTIPLY};

[System.Serializable]
public class StatBuff {
    private string _name;
    public string name {
        get { return _name; }
    }

    public float value;
    public int duration;
    public BuffType type;

    public StatBuff(string name_, float value_ = 0, int duration_ = 0, BuffType type_ = BuffType.ADD) {
        _name = name_;
        value = value_;
        duration = duration_;
        type = type_;
    }
}

public class StatBuffCollection : System.Collections.ObjectModel.Collection<StatBuff> {
    protected override void InsertItem(int index, StatBuff buff) {

        // buffifiers that add are calculated first,
        // so they are inserted at the beginning.
        if (buff.type == BuffType.ADD) {
            base.InsertItem(0, buff);

        // Multiplier buffs are calculated at the end.
        } else {
            base.InsertItem(Count, buff);
        }

        if (buff.duration > 0) {
            Timer timer = new Timer(buff.duration);
            timer.Elapsed += delegate { Remove(buff); };
            timer.Start();
        }
    }
}
