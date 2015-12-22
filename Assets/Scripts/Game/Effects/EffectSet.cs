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
    public int hype = 0;
    public float opinion = 0;

    public GameEvent gameEvent;
    public int eventDelay = 0;
    public float eventProbability = 0;

    public float forgettingRate = 0;
    public float spendingMultiplier = 0;
    public float wageMultiplier = 0;
    public float taxRate = 0;
    public float economicStability = 0;
    public float expansionCostMultiplier = 0;
    public float costMultiplier = 0;

    public List<ProductEffect> productEffects = new List<ProductEffect>();
    public List<StatBuff> workerEffects = new List<StatBuff>();

    public AICompany aiCompany;

    public Special specialEffect;
    public enum Special {
        None,
        Immortal,
        Cloneable,
        Prescient,
        WorkerInsight,
        WorkerQuant,
        FounderAI,
        Automation
    }

    public EffectSet Clone() {
        EffectSet es = new EffectSet();
        es.cash = cash;
        es.unlocks = unlocks;
        es.hype = hype;
        es.opinion = opinion;
        es.gameEvent = gameEvent;
        es.eventDelay = eventDelay;
        es.eventProbability = eventProbability;

        es.forgettingRate = forgettingRate;
        es.spendingMultiplier = spendingMultiplier;
        es.wageMultiplier = wageMultiplier;
        es.taxRate = taxRate;
        es.economicStability = economicStability;

        es.productEffects = new List<ProductEffect>();
        foreach (ProductEffect pe in productEffects) {
            es.productEffects.Add(new ProductEffect(pe));
        }

        es.workerEffects = new List<StatBuff>();
        foreach (StatBuff sb in workerEffects) {
            es.workerEffects.Add(new StatBuff(sb.name, sb.value));
        }

        es.aiCompany = aiCompany;
        es.specialEffect = specialEffect;
        es.expansionCostMultiplier = expansionCostMultiplier;
        es.costMultiplier = costMultiplier;

        return es;
    }

    public void ApplyMultiplier(float mult) {
        forgettingRate *= mult;
        spendingMultiplier *= mult;
        wageMultiplier *= mult;
        taxRate *= mult;
        economicStability *= mult;
        costMultiplier *= mult;
        cash *= mult;

        foreach (ProductEffect pe in productEffects) {
            pe.buff.value *= mult;
        }
        foreach (StatBuff sb in workerEffects) {
            sb.value *= mult;
        }
    }

    public void Apply(Company company) {
        company.activeEffects.Add(this);

        company.cash.baseValue += cash;
        company.opinion += opinion;
        company.hype += hype;

        if (gameEvent != null) {
            AGameEvent ge = new AGameEvent(gameEvent);
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
            foreach (AWorker worker in company.workers) {
                worker.ApplyBuffs(workerEffects);
            }
        }

        if (aiCompany != null)
            AICompany.Find(aiCompany).disabled = false;

        // "World" effects.
        GameManager gm = GameManager.Instance;
        gm.forgettingRate += forgettingRate;
        gm.spendingMultiplier += spendingMultiplier;
        gm.wageMultiplier += wageMultiplier;
        gm.economicStability += economicStability;
        gm.taxRate += taxRate;
        gm.expansionCostMultiplier += expansionCostMultiplier;
        gm.costMultiplier += costMultiplier;

        if (specialEffect != Special.None)
            gm.ApplySpecialEffect(specialEffect);
    }
    public void Remove(Company company) {
        company.activeEffects.Remove(this);

        if (productEffects != null) {
            foreach (ProductEffect fx in productEffects) {
                fx.Remove(company);
            }
        }

        if (workerEffects != null) {
            foreach (AWorker worker in company.workers) {
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

    public void Apply(AWorker worker) {
        if (workerEffects != null)
            worker.ApplyBuffs(workerEffects);
    }

    public void Remove(AWorker worker) {
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

        if (hype != es.hype)
            return false;

        if (opinion != es.opinion)
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
