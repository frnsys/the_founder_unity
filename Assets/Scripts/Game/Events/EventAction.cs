/*
 * An action the player can choose in response to an event.
 */

using UnityEngine;
using System.Linq;
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
        GameEvent ev = RollDependent();
        GameEvent.Trigger(ev);
    }

    // Rolls to see what event happens, assuming that the events are dependent
    // and that only one of them can happen. Probabilities are "normalized",
    // so they can sum up to > 1 and will be handled accordingly.
    private GameEvent RollDependent() {
        System.Random random = new System.Random();
        float total = outcomes.Sum(item => item.probability);
        float roll = (float)(random.NextDouble() * total);
        float min = total;
        GameEvent selectedEvent = null;

        // Find the event which has the probability
        // which minimally satisfies the roll.
        // That is, it has the smallest probability value
        // which is still greater than the roll.
        // For example: probabilities 0.3, 0.7, 1.6 and a 0.6 is rolled.
        // Here, 0.7 is the minimally satisfying probability.
        // The idea is that the probability ranges are 0-0.3, 0.3-0.7, and 0.7-1.6.
        foreach (GameEvent e in outcomes) {
            float prob = e.probability;
            if (roll < prob && prob <= min) {
                selectedEvent = e;
                min = prob;
            }
        }

        return selectedEvent;
    }
}


