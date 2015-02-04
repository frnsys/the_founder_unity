/*
 * A random or triggered game event that has some effects.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject {
    public enum Type {
        Email,
        News
    }

    public static List<GameEvent> LoadSpecialEvents() {
        return Resources.LoadAll<GameEvent>("SpecialEvents").ToList().Select(ev => {
                GameEvent gameEvent = Instantiate(ev) as GameEvent;
                gameEvent.name = ev.name;
                return gameEvent;
        }).ToList();
    }

    public static GameEvent LoadSpecialEvent(string name) {
        GameEvent ev = Resources.Load<GameEvent>("GameEvents/Special/" + name);
        GameEvent clone = Instantiate(ev) as GameEvent;
        clone.name = ev.name;
        return clone;
    }

    public Type type;
    public string description;
    public string from;
    public Texture image;

    [HideInInspector]
    public float delay {
        get { return _delay; }
        set {
            _delay = value;
            countdown = value;
        }
    }
    [SerializeField]
    private float _delay;
    public float countdown;
    public float probability;
    public bool repeatable;

    public EffectSet effects = new EffectSet();
    public List<EventAction> actions = new List<EventAction>();
    public List<Condition> conditions = new List<Condition>();

    public GameEvent(string name_, float probability_) {
        name = name_;
        probability = Mathf.Clamp(probability_, 0, 1);
    }

    // An event which is broadcast for each event.
    static public event System.Action<GameEvent> EventTriggered;
    static public void Trigger(GameEvent ge) {
        // If there are subscribers to this event...
        if (EventTriggered != null) {
            // Broadcast the event.
            EventTriggered(ge);
        }
    }

    public bool ConditionsSatisfied(Company company) {
        foreach (Condition c in conditions) {
            if (!c.Evaluate(company))
                return false;
        }
        return true;
    }
}


