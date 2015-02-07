/*
 * A bundle of different effects,
 * makes managing effects much more convenient.
 *
 * I had a nice system of polymorphism but Unity's serialization
 * system is a bane and doesn't let you do anything like that.
 * HENCE the system here :(
 *
 * Update: Nor does the Unity serializer support null values for fields
 * in custom classes. Why?? So custom class fields here can't be null,
 * even if you want it to be. Unity will forcefully instantiate
 * a new instance for that field if you try to set it as null.
 *
 * So instead of literal nulls, we have null-equivalent values,
 * e.g. a StatBuff with a value of 0.
 *
 * See: <http://blogs.unity3d.com/2014/06/24/serialization-in-unity/>
 */

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EffectSet {
    public UnlockSet unlocks = new UnlockSet();

    public float cash = 0;

    // To keep things consistent with Unity's serialization,
    // these values are initialized as non-null.
    public StatBuff research = new StatBuff("Research", 0);
    public OpinionEvent opinionEvent = new OpinionEvent(0, 0);

    public GameEvent gameEvent;
    public float eventDelay = 0;
    public float eventProbability = 0;

    public List<ProductEffect> productEffects;
    public List<StatBuff> workerEffects;

    public AICompany aiCompany;

    public void Apply(Company company) {
        company.activeEffects.Add(this);

        company.cash.baseValue += cash;

        if (research.value != 0)
            company.research.ApplyBuff(research);

        if (opinionEvent.opinion.value != 0 && opinionEvent.publicity.value != 0)
            company.ApplyOpinionEvent(opinionEvent);

        if (gameEvent != null) {
            GameEvent ge = ScriptableObject.Instantiate(gameEvent) as GameEvent;
            ge.delay = eventDelay;
            ge.probability = eventProbability;
            GameManager.Instance.eventManager.Add(ge);
        }

        if (productEffects != null) {
            foreach (ProductEffect fx in productEffects) {
                fx.Apply(company);
            }
        }

        if (workerEffects != null) {
            foreach (Worker worker in company.workers) {
                worker.ApplyBuffs(workerEffects);
            }
        }

        if (aiCompany != null)
            AICompany.Find(aiCompany).disabled = false;
    }
    public void Remove(Company company) {
        company.activeEffects.Remove(this);

        if (research.value != 0)
            company.research.RemoveBuff(research);

        if (productEffects != null) {
            foreach (ProductEffect fx in productEffects) {
                fx.Remove(company);
            }
        }

        if (workerEffects != null) {
            foreach (Worker worker in company.workers) {
                worker.RemoveBuffs(workerEffects);
            }
        }

        // If the event is repeatable, we remove it.
        // Otherwise, let it resolve eventually.
        if (gameEvent != null && gameEvent.repeatable)
            GameManager.Instance.eventManager.Remove(gameEvent);

        // Cash effects don't have a reverse.
        // No removing of opinion events,
        // they degrade naturally via "forgetting".
    }

    public void Apply(Worker worker) {
        if (workerEffects != null)
            worker.ApplyBuffs(workerEffects);
    }

    public void Remove(Worker worker) {
        if (workerEffects != null)
            worker.RemoveBuffs(workerEffects);
    }

    public void Apply(Product product) {
        if (productEffects != null) {
            foreach (ProductEffect fx in productEffects) {
                fx.Apply(product);
            }
        }
    }

    public void Remove(Product product) {
        if (productEffects != null) {
            foreach (ProductEffect fx in productEffects) {
                fx.Remove(product);
            }
        }
    }

    public bool Equals(EffectSet es) {
        if (es == null)
            return false;

        if (cash != es.cash)
            return false;

        if (!research.Equals(es.research))
            return false;

        if (!opinionEvent.Equals(es.opinionEvent))
            return false;

        if (gameEvent != es.gameEvent)
            return false;
        if (eventDelay != es.eventDelay)
            return false;
        if (eventProbability != es.eventProbability)
            return false;

        if (productEffects.Count != es.productEffects.Count)
            return false;

        for (int i=0; i<productEffects.Count; i++) {
            if (!productEffects[i].Equals(es.productEffects[i]))
                return false;
        }

        if (workerEffects.Count != es.workerEffects.Count)
            return false;

        for (int i=0; i<workerEffects.Count; i++) {
            if (!workerEffects[i].Equals(es.workerEffects[i]))
                return false;
        }

        return true;
    }
}
