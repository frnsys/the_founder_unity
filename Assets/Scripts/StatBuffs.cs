using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public enum BuffType {add, multiply};

public class StatBuffs : MonoBehaviour {
    public List<StatBuff> buffs = new List<StatBuff>();
}

public class StatBuff {
    public string name;
    public float value;
    public int duration;
    public BuffType type;

    public StatBuff(string name_, float value_, int duration_ = 0, BuffType type_ = BuffType.add) {
        name = name_;
        value = value_;
        duration = duration_;
        type = type_;
    }
}

public class StatBuffCollection : System.Collections.ObjectModel.Collection<StatBuff> {
    protected override void InsertItem(int index, StatBuff buff) {

        // buffifiers that add are calculated first,
        // so they are inserted at the beginning.
        if (buff.type == BuffType.add) {
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
