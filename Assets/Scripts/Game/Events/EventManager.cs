/*
 * Manages events, in particular, events with unique effects (beyond those supported by EffectSet).
 * Note: Events should NOT be handled via coroutines, because those don't save their state.
 * So between saves, we may lose events which were supposed to be triggered.
 * Instead, rely on `event ticks`.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
    private GameData data;

    public void Load(GameData d) {
        data = d;
    }

    public void Add(AGameEvent ev) {
        data.eventsPool.Add(ev);
    }

    public void Remove(AGameEvent ev) {
        // Remove a given event by name.
        int idx = data.eventsPool.FindIndex(e => e.name == ev.name);
        data.eventsPool.RemoveAt(idx);
    }

    public void Remove(GameEvent ev) {
        // Remove a given event by name.
        int idx = data.eventsPool.FindIndex(e => e.name == ev.name);
        data.eventsPool.RemoveAt(idx);
    }

    public void Tick() {
        List<AGameEvent> toResolve = new List<AGameEvent>();

        foreach (AGameEvent ev_ in data.eventsPool) {
            ev_.countdown -= 1;

            if (ev_.countdown <= 0)
                toResolve.Add(ev_);
        }

        // If there is one event to resolve, resolve it.
        AGameEvent ev;
        if (toResolve.Count == 1) {
            ev = toResolve[0];
            if (Random.value <= ev.probability)
                GameEvent.Trigger(ev.gameEvent);
            data.eventsPool.Remove(ev);

        // If there are more, randomly pick events until one is triggered
        // or none are left.
        } else if (toResolve.Count > 1) {
            while (toResolve.Count > 0) {
                ev = toResolve[Random.Range(0, toResolve.Count)];

                toResolve.Remove(ev);

                // Repeatable events remain in the pool.
                if (!ev.gameEvent.repeatable) {
                    data.eventsPool.Remove(ev);
                } else {
                    // Reset the delay/countdown.
                    ev.countdown = ev.delay;
                }

                if (Random.value <= ev.probability) {
                    GameEvent.Trigger(ev.gameEvent);
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
        foreach (AGameEvent ev in data.specialEventsPool) {
            // Only show one event, the next one will
            // resolve in the following iteration.
            if (ev.gameEvent.ConditionsSatisfied(data.company)) {
                // We trigger the event by adding it to the regular event pool.
                ev.probability = 1f;

                // This is in number of weeks.
                DelayTrigger(ev, 5f);

                // Remove it now though so that it doesn't re-trigger.
                if (!ev.gameEvent.repeatable)
                    data.specialEventsPool.Remove(ev);
                break;
            }
        }
    }

    // It's expected that the event passed in is a clone.
    public void DelayTrigger(AGameEvent ge, float ticks) {
        ge.countdown = ticks;
        Add(ge);
    }
}
