/*
 * A wrapper around GameEvents which includes stuff which needs to be saved.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AGameEvent {
    public GameEvent gameEvent;
    public float probability;

    public AGameEvent() {}
    public AGameEvent(GameEvent ev) {
        gameEvent = ev;
    }

    public string name {
        get { return gameEvent.name; }
    }
}


