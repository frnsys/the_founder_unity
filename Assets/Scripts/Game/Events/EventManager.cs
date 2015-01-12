/*
 * Manages events, in particular, events with unique effects (beyond those supported by EffectSet).
 */

using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
    private GameData data;

    public void Load(GameData d) {
        data = d;
    }

    public void Add(GameEvent ev) {
        data.eventsPool.Add(ev);
    }

    public void Tick() {
        List<GameEvent> toResolve = new List<GameEvent>();

        foreach (GameEvent ev_ in data.eventsPool) {
            ev_.delay -= 1;

            if (ev_.delay == 0)
                toResolve.Add(ev_);
        }

        // If there is one event to resolve, resolve it.
        GameEvent ev;
        if (toResolve.Count == 1) {
            ev = toResolve[0];
            if (Random.value <= ev.probability)
                GameEvent.Trigger(ev);
            data.eventsPool.Remove(ev);

        // If there are more,
        } else if (toResolve.Count > 1) {
            while (toResolve.Count > 0) {
                ev = toResolve[Random.Range(0, toResolve.Count)];

                toResolve.Remove(ev);
                data.eventsPool.Remove(ev);

                if (Random.value <= ev.probability) {
                    GameEvent.Trigger(ev);
                    break;
                }
            }

            // Any events we didn't get to, up their delay so
            // we can try again later.
            for (int i=0; i < toResolve.Count; i++) {
                toResolve[i].delay = Random.Range(4, 20);
            }
        }
    }
}
