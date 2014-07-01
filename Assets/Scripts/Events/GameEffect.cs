using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEffect {
    public enum Type {
        CASH,
        ECONOMY,
        PRODUCT,
        WORKER,
        EVENT,
        UNLOCK
    }
    public Type type;
    public string subtype;
    public float amount;
    public string stat;
    public int id;

    public GameEffect(Type type_,
            string subtype_ = null,
            string stat_ = null,
            float amount_ = 0f,
            int id_ = 0)
    {
        type = type_;
        subtype = subtype_;
        stat = stat_;
        amount = amount_;
        id = id_;
    }
}
