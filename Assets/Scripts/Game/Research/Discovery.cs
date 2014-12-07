/*
 * Discoveries are the output of a consultancy's research.
 * They can have various effects, such as unlocking things.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Discovery : ScriptableObject {
    public string description;

    public Research requiredResearch = new Research(100,100,100);

    public EffectSet effects = new EffectSet();
}
