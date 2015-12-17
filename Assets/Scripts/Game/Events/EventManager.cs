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
}
