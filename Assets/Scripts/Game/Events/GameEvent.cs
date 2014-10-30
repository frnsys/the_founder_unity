using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject, IUnlockable {

    // Roll to see what events happen, out of a set of specified
    // candidate events.
    public static void Roll(List<GameEvent> candidateEvents) {
        // Have to iterate this way, since
        // the candidateEvents list may change while this is iterating.
        for (int i=0; i<candidateEvents.Count; i++) {
            // Does this event happen?
            if (Random.value < candidateEvents[i].probability.value) {
                Trigger(candidateEvents[i]);
            }

        }
    }

    // Rolls to see what event happens, assuming that the events are dependent
    // and that only one of them can happen. Probabilities are "normalized",
    // so they can sum up to > 1 and will be handled accordingly.
    public static void RollDependent(List<GameEvent> candidateEvents) {
        System.Random random = new System.Random();
        float total = candidateEvents.Sum(item => item.probability.value);
        float roll = (float)(random.NextDouble() * total);
        float min = total;
        GameEvent selectedEvent = null;

        // Find the event which has the probability
        // which minimally satisfies the roll.
        // That is, it has the smallest probability value
        // which is still greater than the roll.
        // For example: probabilities 0.3, 0.7, 1.6 and a 0.6 is rolled.
        // Here, 0.7 is the minimally satisfying probability.
        // The idea is that the probability ranges are 0-0.3, 0.3-0.7, and 0.7-1.6.
        foreach (GameEvent e in candidateEvents) {
            float prob = e.probability.value;
            if (roll < prob && prob <= min) {
                selectedEvent = e;
                min = prob;
            }
        }

        Trigger(selectedEvent);
    }




    public string description;
    public bool repeatable;
    public Stat probability;

    public EffectSet effects = new EffectSet();

    public List<EventAction> actions = new List<EventAction>();

    public GameEvent(string name_, float probability_, bool repeatable_) {
        name = name_;
        repeatable = repeatable_;

        probability_ = Mathf.Clamp(probability_, 0, 1);
        probability = new Stat("Probability", probability_);
    }

    // An event which is broadcast for each event effect.
    static public event System.Action<GameEvent> EventTriggered;
    static public void Trigger(GameEvent ge) {
        // If there are subscribers to this event...
        if (EventTriggered != null) {
            // Broadcast the event.
            EventTriggered(ge);
        }
    }
}


