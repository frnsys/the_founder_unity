/*
 * A public opinion event.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class OpinionEvent {
    public string name;
    public StatBuff opinion;
    public StatBuff publicity;

    public OpinionEvent() {
        opinion   = new StatBuff("Opinion", 0);
        publicity = new StatBuff("Hype", 0);
    }
    public OpinionEvent(float o, float p) {
        opinion   = new StatBuff("Opinion", o);
        publicity = new StatBuff("Hype", p);
    }

    // "Forget" the event, i.e. pull back its effect towards 0.
    public void Forget(float rate) {
        if (opinion.value > 0) {
            opinion.value -= rate;

            // If overshot, just set to 0.
            if (opinion.value < 0)
                opinion.value = 0;
        } else if (opinion.value < 0) {
            opinion.value += rate;

            // If overshot, just set to 0.
            if (opinion.value > 0)
                opinion.value = 0;
        }
    }

    public bool Equals(OpinionEvent oe) {
        return name != oe.name &&
                !opinion.Equals(oe.opinion) &&
                !publicity.Equals(oe.publicity);
    }
}
