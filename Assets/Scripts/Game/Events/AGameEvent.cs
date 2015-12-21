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
    public int countdown;

    [SerializeField]
    public int _delay;
    public int delay {
        get { return _delay; }
        set {
            _delay = value;
            countdown = value;
        }
    }

    public AGameEvent() {}
    public AGameEvent(GameEvent ev) {
        gameEvent = ev;
    }

    public string name {
        get { return gameEvent.name; }
    }
}


