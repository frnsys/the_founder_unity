using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public enum ModifierType {add, multiply};

public class StatModifier {
    public string name;
    public float value;
    public int duration;
    public ModifierType type;

    public StatModifier(string name_, float value_, int duration_ = 0, ModifierType type_ = ModifierType.add) {
        name = name_;
        value = value_;
        duration = duration_;
        type = type_;
    }
}

public class StatModifierCollection : System.Collections.ObjectModel.Collection<StatModifier> {
    protected override void InsertItem(int index, StatModifier mod) {

        // Modifiers that add are calculated first,
        // so they are inserted at the beginning.
        if (mod.type == ModifierType.add) {
            base.InsertItem(0, mod);

        // Multiplier mods are calculated at the end.
        } else {
            base.InsertItem(Count, mod);
        }

        if (mod.duration > 0) {
            Timer timer = new Timer(mod.duration);
            timer.Elapsed += delegate { Remove(mod); };
            timer.Start();
        }
    }
}
