/*
 * A bundle of different effects,
 * makes managing effects much more convenient.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EffectSet : IEffect {
    public List<ProductEffect> products     = new List<ProductEffect>();
    public List<StatBuff> workers           = new List<StatBuff>();
    public List<StatBuff> company           = new List<StatBuff>();
    public UnlockSet unlocks                = new UnlockSet();
    public List<OpinionEvent> opinionEvents = new List<OpinionEvent>();

    public List<IEffect> effects = new List<IEffect>();
    public void Apply(Company company) {
        foreach (IEffect e in effects) {
            e.Apply(company);
        }
    }
    public void Remove(Company company) {
        foreach (IEffect e in effects) {
            e.Remove(company);
        }
    }
}
