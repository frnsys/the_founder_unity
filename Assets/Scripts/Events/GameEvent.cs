using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class GameEvent : ScriptableObject {

    // Roll to see what events happen, out of a set of specified
    // candidate events.
    public static List<GameEvent> Roll(List<GameEvent> candidateEvents) {
        List<GameEvent> theseAreHappening = new List<GameEvent>();

        foreach (GameEvent gameEvent in candidateEvents) {

            // Does this event happen?
            if (UnityEngine.Random.value < gameEvent.probability.value) {
                theseAreHappening.Add(gameEvent);
            }

        }
        return theseAreHappening;
    }


    public string name;
    public Stat probability;

    // Effects
    public List<ProductEffect> productEffects = new List<ProductEffect>();
    public List<WorkerEffect> workerEffects = new List<WorkerEffect>();
    public List<CompanyEffect> companyEffects = new List<CompanyEffect>();
    public List<EventEffect> eventEffects = new List<EventEffect>();
    public List<EconomyEffect> economyEffects = new List<EconomyEffect>();
    public List<UnlockEffect> unlockEffects = new List<UnlockEffect>();

    public GameEvent(string name_, float probability_) {
        name = name_;

        // Maximum probability is 1,
        // minimum is 0.
        probability_ = Mathf.Clamp(probability_, 0, 1);
        probability = new Stat("Probability", probability_);
    }


    // An event which is broadcast for each event effect.
    public event System.Action<GameEffect, Type> EffectEvent;
    public void Trigger() {
        // If there are subscribers to this event...
        if (EffectEvent != null) {
            // What a travesty
            List<GameEffect> effects = productEffects.Cast<GameEffect>()
                                        .Concat(workerEffects.Cast<GameEffect>())
                                        .Concat(companyEffects.Cast<GameEffect>())
                                        .Concat(eventEffects.Cast<GameEffect>())
                                        .Concat(economyEffects.Cast<GameEffect>())
                                        .Concat(unlockEffects.Cast<GameEffect>())
                                        .ToList();

            foreach (GameEffect effect in effects) {
                // Broadcast the event with the effect.
                EffectEvent(effect, effect.GetType());
            }
        }
    }
}


