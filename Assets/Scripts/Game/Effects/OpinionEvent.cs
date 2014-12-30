/*
 * A public opinion event.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class OpinionEvent {
    public string name;
    public StatBuff effect;

    public OpinionEvent(float value) {
        effect = new StatBuff("Opinion", value, 0);
    }

    // "Forget" the event, i.e. pull back its effect towards 0.
    public void Forget(float rate) {
        if (effect.value > 0) {
            effect.value -= rate;

            // If overshot, just set to 0.
            if (effect.value < 0)
                effect.value = 0;
        } else if (effect.value < 0) {
            effect.value += rate;

            // If overshot, just set to 0.
            if (effect.value > 0)
                effect.value = 0;
        }
    }
}
