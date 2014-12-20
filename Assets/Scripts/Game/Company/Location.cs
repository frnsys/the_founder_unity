using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : ScriptableObject {
    public string description;

    public float cost = 1000000;

    public Infrastructure capacity = new Infrastructure();
    public EffectSet effects = new EffectSet();
}
