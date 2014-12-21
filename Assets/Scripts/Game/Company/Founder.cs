/*
 * Founders are special workers which have special bonuses.
 */

using UnityEngine;

[System.Serializable]
public class Founder : Worker {
    public EffectSet bonuses;

    public virtual void Awake() {
        bonuses = new EffectSet();
    }
}
