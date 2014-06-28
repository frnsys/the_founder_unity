using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public StatBuff(string name_, float value_, int duration_ = 0, BuffType type_ = BuffType.ADD) {
        _name = name_;
        value = value_;
        duration = duration_;
        type = type_;
    }
}
