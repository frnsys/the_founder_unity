using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EventAction {
    public string name;
    public List<GameEvent> outcomes;

    public EventAction(string name_, List<GameEvent> outcomes_) {
        name = name_;
        outcomes = outcomes_;
    }

    static public event System.Action<EventAction> ActionTriggered;
    static public void Trigger(EventAction ea) {
        if (ActionTriggered != null) {
            ActionTriggered(ea);
        }
    }
}


