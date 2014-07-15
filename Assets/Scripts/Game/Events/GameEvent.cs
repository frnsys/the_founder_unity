using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject {

    // Roll to see what events happen, out of a set of specified
    // candidate events.
    public static List<GameEvent> Roll(List<GameEvent> candidateEvents) {
        List<GameEvent> theseAreHappening = new List<GameEvent>();

        foreach (GameEvent gameEvent in candidateEvents) {

            // Does this event happen?
            if (Random.value < gameEvent.probability.value) {
                theseAreHappening.Add(gameEvent);
            }

        }
        return theseAreHappening;
    }

    public string description;
    public Stat probability;
    public List<GameEffect> effects = new List<GameEffect>();

    public GameEvent(string name_, float probability_) {
        name = name_;

        // Maximum probability is 1,
        // minimum is 0.
        probability_ = Mathf.Clamp(probability_, 0, 1);
        probability = new Stat("Probability", probability_);
    }

    // An event which is broadcast for each event effect.
    public event System.Action<GameEffect> EffectEvent;
    public void Trigger() {
        // If there are subscribers to this event...
        if (EffectEvent != null) {
            foreach (GameEffect effect in effects) {
                // Broadcast the event with the effect.
                EffectEvent(effect);
            }
        }
    }
}


