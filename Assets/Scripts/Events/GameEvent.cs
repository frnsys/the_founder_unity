using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

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


    public string name;
    public Stat probability;
    public List<GameEffect> effects = new List<GameEffect>();

    public GameEvent(JSONClass prototype) {
        // TO DO So ugly. If we move to a more sophisticated
        // event management system, maybe we can clean this up.
        JSONArray effects__ = prototype["effects"].AsArray;
        List<GameEffect> effects_ = new List<GameEffect>();
        foreach (JSONNode effect in effects__) {
            string type_ = effect["type"];
            GameEffect.Type type = (GameEffect.Type)System.Enum.Parse(typeof(GameEffect.Type), type_, true);

            string subtype = effect["subtype"];
            string stat = effect["stat"];
            float amount = effect["amount"].AsFloat;
            int id = effect["id"].AsInt;
            effects_.Add(new GameEffect(type, subtype, stat, amount, id));
        }

        Initialize(prototype["name"], prototype["probability"].AsFloat, effects_);
    }
    public GameEvent(string name_, float probability_) {
        Initialize(name_, probability_, new List<GameEffect>());
    }
    public void Initialize(string name_, float probability_, List<GameEffect> effects_) {
        name = name_;
        effects = effects_;

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


