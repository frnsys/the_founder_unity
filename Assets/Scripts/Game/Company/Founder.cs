/*
 * Founders are special workers which have special bonuses.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Founder : Worker {
    public EffectSet bonuses;

    void Start() {
        bonuses = new EffectSet();
    }
}
