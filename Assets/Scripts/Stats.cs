using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stats : MonoBehaviour {

    public List<Stat> stats = new List<Stat>();

    public void LoadBuffs(StatBuffs sb) {
        foreach (StatBuff buff in sb.buffs) {
            // Find a stat who's name matches this buff's name.
            Stat stat = Get(buff.name);

            // Add the buff to the stat.
            // (stat.buffs is a list of buffs)
            stat.buffs.Add(buff);
        }
    }
    public void UnloadBuffs(StatBuffs sb) {
        foreach (StatBuff buff in sb.buffs) {
            Stat stat = Get(buff.name);
            stat.buffs.Remove(buff);
        }
    }

    // Get a Stat by name.
    public Stat Get(string name) {
        return stats.Find(i => i.name == name);
    }
}

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
