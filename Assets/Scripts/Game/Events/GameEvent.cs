/*
 * A random or triggered game event that has some effects.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject {
    public static List<GameEvent> LoadSpecialEvents() {
        // TO DO replace with actual loading
        return new List<GameEvent>();
    }

    public string description;
    public float probability;
    public float delay;

    public EffectSet effects = new EffectSet();
    public List<EventAction> actions = new List<EventAction>();

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
}


