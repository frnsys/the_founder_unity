using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameEvent {

    public enum Type {
        CASH,
        ECONOMY,
        PRODUCT,
        WORKER,
        EVENT,
        UNLOCK
    }

    public struct Effect {
        public Type type;
        public string subtype;
        public float amount;
        public string stat;
        public int id;

        public Effect(Type type_,
                string subtype_ = null,
                string stat_ = null,
                float amount_ = 0f,
                int id_ = 0)
        {
            type = type_;
            subtype = subtype_;
            stat = stat_;
            amount = amount_;
            id = id_;
        }
    }

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
    public List<Effect> effects = new List<Effect>();

    public GameEvent(JSONClass prototype) {
        // TO DO So ugly. If we move to a more sophisticated
        // event management system, maybe we can clean this up.
        JSONArray effects__ = prototype["effects"].AsArray;
        List<Effect> effects_ = new List<Effect>();
        foreach (JSONNode effect in effects__) {
            string type_ = effect["type"];
            Type type = (Type)System.Enum.Parse(typeof(Type), type_);

            string subtype = effect["subtype"];
            string stat = effect["stat"];
            float amount = effect["amount"].AsFloat;
            int id = effect["id"].AsInt;
            effects_.Add(new Effect(type, subtype, stat, amount, id));
        }

        Initialize(prototype["name"], prototype["probability"].AsFloat, effects_);
    }
    public GameEvent(string name_, float probability_) {
        Initialize(name_, probability_, new List<Effect>());
    }
    public void Initialize(string name_, float probability_, List<Effect> effects_) {
        name = name_;
        effects = effects_;

        // Maximum probability is 1,
        // minimum is 0.
        probability_ = Mathf.Clamp(probability_, 0, 1);
        probability = new Stat("Probability", probability_);
    }



    // An event which is broadcast for each event effect.
    public event System.Action<Effect> EventEffect;
    public void Trigger() {
        // If there are subscribers to this event...
        if (EventEffect != null) {
            foreach (Effect effect in effects) {
                // Broadcast the event with the effect.
                EventEffect(effect);
            }
        }
    }
}


