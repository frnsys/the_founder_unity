/*
 * A random or triggered game event that has some effects.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : ScriptableObject {
    public enum Type {
        Email,
        News,
        Personal
    }

    public static List<GameEvent> LoadSpecialEvents() {
        return Resources.LoadAll<GameEvent>("SpecialEvents").ToList().Select(ev => {
            GameEvent gameEvent = Instantiate(ev) as GameEvent;
            gameEvent.name = ev.name;
            return gameEvent;
        }).ToList();
    }

    public static GameEvent LoadSpecialEvent(string name) {
        GameEvent ev = Resources.Load<GameEvent>("SpecialEvents/" + name);
        GameEvent clone = Instantiate(ev) as GameEvent;
        clone.name = ev.name;
        return clone;
    }

    public static GameEvent LoadNoticeEvent(string name) {
        GameEvent ev = Resources.Load<GameEvent>("NoticeEvents/" + name);
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
    public Action[] actions;
    public Condition[] conditions;

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
    static public IEnumerator DelayTrigger(GameEvent ge, float seconds) {
        yield return new WaitForSeconds(seconds);
        Trigger(ge);
    }

    public bool ConditionsSatisfied(Company company) {
        for (int i=0; i < conditions.Length; i++) {
            if (!EvaluateCondition(company, conditions[i]))
                return false;
        }
        return true;
    }
    private bool EvaluateCondition(Company c, Condition cond) {
        float comparison = 0;
        switch (cond.type) {
            case Condition.Type.Publicity:
                comparison = c.publicity.value;
                break;
            case Condition.Type.Opinion:
                comparison = c.opinion.value;
                break;
            case Condition.Type.Cash:
                comparison = c.cash.value;
                break;
            case Condition.Type.AnnualRevenue:
                comparison = c.annualRevenue;
                break;
            case Condition.Type.Date:
                comparison = GameManager.Instance.date;
                break;
            case Condition.Type.ProductsLaunched:
                comparison = c.launchedProducts.Count;
                break;
            case Condition.Type.Locations:
                comparison = c.locations.Count;
                break;
            case Condition.Type.Employees:
                comparison = c.employeesAcrossLocations;
                break;
            case Condition.Type.OfficeLevel:
                comparison = (int)c.office;
                break;
            case Condition.Type.TechnologiesResearched:
                comparison = c.technologies.Count;
                break;
            case Condition.Type.DeathToll:
                comparison = c.deathToll;
                break;
            case Condition.Type.Pollution:
                comparison = c.pollution;
                break;
        }

        if (cond.greater)
            return comparison > cond.value;
        else
            return comparison < cond.value;
    }

    [System.Serializable]
    public class Condition {
        public float value;
        public bool greater;
        public Type type;

        public enum Type {
            Publicity,
            Opinion,
            Cash,
            AnnualRevenue,
            Date,
            ProductsLaunched,
            Locations,
            Employees,
            OfficeLevel,
            TechnologiesResearched,
            DeathToll,
            Pollution
        }

        public Condition() {}
    }

    [System.Serializable]
    public class Action {
        public string name;
        public EffectSet effects;

        public Action() {}
    }
}


