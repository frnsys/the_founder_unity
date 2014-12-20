using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : ScriptableObject {
    public string description;

    public float cost = 1000000;

    // Infrastructure
    public int capacity = 10;

    public EffectSet effects = new EffectSet();
}
