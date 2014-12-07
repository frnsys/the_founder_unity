/*
 * An action the player can choose in response to an event.
 */

using UnityEngine;
using System.Timers;
using System.Collections.Generic;

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


