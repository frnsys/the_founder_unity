/*
 * A random or triggered game event that has some effects.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEvent : SharedResource<GameEvent> {
    public enum Type {
        Email,
        News,
        Personal
    }

    public static List<AGameEvent> LoadSpecialEvents() {
        return Resources.LoadAll<GameEvent>("SpecialEvents").ToList().Select(ev => {
            return new AGameEvent(ev);
        }).ToList();
    }

    public static AGameEvent LoadSpecialEvent(string name) {
        GameEvent ev = Resources.Load<GameEvent>("SpecialEvents/" + name);
        return new AGameEvent(ev);
    }

    public static AGameEvent LoadNoticeEvent(string name) {
        GameEvent ev = Resources.Load<GameEvent>("NoticeEvents/" + name);
        return new AGameEvent(ev);
    }

    public static new GameEvent Load(string name) {
        GameEvent ev = Resources.Load("NoticeEvents/" + name) as GameEvent;
        if (ev == null) {
            ev = Resources.Load("SpecialEvents/" + name) as GameEvent;
        }
        return ev;
    }

    public Type type;
    public string description;
    public string from;
    public Texture image;
    public bool repeatable;

    public EffectSet effects = new EffectSet();
    public List<Action> actions;
    public List<Condition> conditions;

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
        for (int i=0; i < conditions.Count; i++) {
            if (!EvaluateCondition(company, conditions[i]))
                return false;
        }
        return true;
    }
    private bool EvaluateCondition(Company c, Condition cond) {
        float comparison = 0;
        switch (cond.type) {
            case Condition.Type.Hype:
                comparison = c.hype;
                break;
            case Condition.Type.Opinion:
                comparison = c.opinion;
                break;
            case Condition.Type.Cash:
                comparison = c.cash.value;
                break;
            case Condition.Type.AnnualRevenue:
                comparison = c.annualRevenue;
                break;
            case Condition.Type.Year:
                comparison = GameManager.Instance.year;
                break;
            case Condition.Type.ProductsLaunched:
                comparison = c.launchedProducts;
                break;
            case Condition.Type.Locations:
                comparison = c.locations.Count;
                break;
            case Condition.Type.Employees:
                comparison = c.workers.Count;
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
            case Condition.Type.Debt:
                comparison = c.debtOwned;
                break;
            case Condition.Type.Distractedness:
                comparison = GameManager.Instance.forgettingRate;
                break;
            case Condition.Type.Wages:
                comparison = GameManager.Instance.wageMultiplier;
                break;
            case Condition.Type.Spending:
                comparison = GameManager.Instance.spendingMultiplier;
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
            Hype,
            Opinion,
            Cash,
            AnnualRevenue,
            Year,
            ProductsLaunched,
            Locations,
            Employees,
            OfficeLevel,
            TechnologiesResearched,
            DeathToll,
            Pollution,
            Debt,
            Distractedness,
            Wages,
            Spending
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


