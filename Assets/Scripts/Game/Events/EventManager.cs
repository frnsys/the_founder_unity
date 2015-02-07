/*
 * Manages events, in particular, events with unique effects (beyond those supported by EffectSet).
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
    private GameData data;

    public void Load(GameData d) {
        data = d;
    }

    public void Add(GameEvent ev) {
        data.eventsPool.Add(ev);
    }

    public void Remove(GameEvent ev) {
        // Remove a given event by name.
        int idx = data.eventsPool.FindIndex(e => e.name == ev.name);
        data.eventsPool.RemoveAt(idx);
    }

    public void Tick() {
        List<GameEvent> toResolve = new List<GameEvent>();

        foreach (GameEvent ev_ in data.eventsPool) {
            ev_.countdown -= 1;

            if (ev_.countdown <= 0)
                toResolve.Add(ev_);
        }

        // If there is one event to resolve, resolve it.
        GameEvent ev;
        if (toResolve.Count == 1) {
            ev = toResolve[0];
            if (Random.value <= ev.probability)
                GameEvent.Trigger(ev);
            data.eventsPool.Remove(ev);

        // If there are more, randomly pick events until one is triggered
        // or none are left.
        } else if (toResolve.Count > 1) {
            while (toResolve.Count > 0) {
                ev = toResolve[Random.Range(0, toResolve.Count)];

                toResolve.Remove(ev);

                // Repeatable events remain in the pool.
                if (!ev.repeatable) {
                    data.eventsPool.Remove(ev);
                } else {
                    // Reset the delay/countdown.
                    ev.countdown = ev.delay;
                }

                if (Random.value <= ev.probability) {
                    GameEvent.Trigger(ev);
                    break;
                }
            }

            // Any events we didn't get to, up their countdown so
            // we can try again later.
            for (int i=0; i < toResolve.Count; i++) {
                toResolve[i].countdown = Random.Range(4, 20);
            }
        }
    }

    // Evaluate special events to see if conditions have been met.
    public void EvaluateSpecialEvents() {
        foreach (GameEvent ev in data.specialEventsPool) {
            // Only show one event, the next one will
            // resolve in the following iteration.
            if (ev.ConditionsSatisfied(data.company)) {
                // Trigger the event, with a delay.
                StartCoroutine(TriggerSpecialEvent(ev));
                break;
            }
        }
    }

    // Trigger special events with a delay.
    IEnumerator TriggerSpecialEvent(GameEvent ev) {
        yield return new WaitForSeconds(45f);
        GameEvent.Trigger(ev);
        data.specialEventsPool.Remove(ev);
    }
}
