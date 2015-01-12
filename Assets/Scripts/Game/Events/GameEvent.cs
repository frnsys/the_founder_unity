/*
 * A random or triggered game event that has some effects.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject {
    public static List<GameEvent> LoadSpecialEvents() {
        return Resources.LoadAll<GameEvent>("SpecialEvents").ToList().Select(ev => {
                GameEvent gameEvent = Instantiate(ev) as GameEvent;
                gameEvent.name = ev.name;
                return gameEvent;
        }).ToList();
    }

    public string description;

    [HideInInspector]
    public float delay;
    public float probability;

    public EffectSet effects = new EffectSet();
    public List<EventAction> actions = new List<EventAction>();
    public List<Condition> conditions = new List<Condition>();

    public GameEvent(string name_, float probability_) {
        name = name_;
        probability = Mathf.Clamp(probability_, 0, 1);
    }

    // An event which is broadcast for each event.
    static public event System.Action<GameEvent> EventTriggered;
    static public void Trigger(GameEvent ge) {
        // If there are subscribers to this event...
        if (EventTriggered != null) {
            // Broadcast the event.
            EventTriggered(ge);
        }
    }

    public bool ConditionsSatisfied(Company company) {
        foreach (Condition c in conditions) {
            if (!c.Evaluate(company))
                return false;
        }
        return true;
    }
}


