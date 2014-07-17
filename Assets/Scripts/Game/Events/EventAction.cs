using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

[System.Serializable]
public class EventAction {
    public string name;
    public int delay;
    public List<GameEvent> outcomes;

    public EventAction(string name_, List<GameEvent> outcomes_, int delay_) {
        name = name_;
        outcomes = outcomes_;
        delay = delay_;
    }

    // Execute the action,
    // resolving it after a delay if one is specified,
    // otherwise, it is resolved immediately.
    public void Execute() {
        if (delay > 0) {
            Debug.Log(delay);
            Timer timer  = new Timer(delay);
            timer.Elapsed += delegate {
                Resolve();
                timer.Stop();
                timer = null;
            };
            timer.Start();
        } else {
            Resolve();
        }
    }

    public void Resolve() {
        // Roll to see what outcome happens
        // and trigger it.
        GameEvent.RollDependent(outcomes);
    }
}


