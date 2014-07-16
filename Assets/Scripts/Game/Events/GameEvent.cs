using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject, IUnlockable {

    // Roll to see what events happen, out of a set of specified
    // candidate events.
    public static void Roll(List<GameEvent> candidateEvents) {
        foreach (GameEvent gameEvent in candidateEvents) {

            // Does this event happen?
            if (Random.value < gameEvent.probability.value) {
                Trigger(gameEvent);
            }

        }
    }

    public string name;
    public string description;

    public Stat probability;

    public List<ProductEffect> productEffects = new List<ProductEffect>();
    public List<StatBuff> workerEffects = new List<StatBuff>();
    public List<StatBuff> companyEffects = new List<StatBuff>();
    public UnlockSet unlocks = new UnlockSet();

    public List<EventAction> actions = new List<EventAction>();

    public GameEvent(string name_, float probability_) {
        name = name_;

        // Maximum probability is 1,
        // minimum is 0.
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


