/*
 * A bundle of different effects,
 * makes managing effects much more convenient.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectSet {
    public List<ProductEffect> products = new List<ProductEffect>();
    public List<StatBuff> workers       = new List<StatBuff>();
    public List<StatBuff> company       = new List<StatBuff>();
    public UnlockSet unlocks            = new UnlockSet();
}
