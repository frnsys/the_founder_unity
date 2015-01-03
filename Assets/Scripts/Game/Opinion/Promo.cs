using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : ScriptableObject {
    public OpinionEvent opinionEvent;
    public float cost;
    public float requiredProgress;
    public float difficulty;

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    void Awake() {
        opinionEvent = new OpinionEvent(0, 0);
        difficulty   = 1;
    }

    static public event System.Action<Promo> Completed;
    public bool Develop(float amount, float skill) {
        _progress += amount;
        if (progress >= 1) {
            opinionEvent.opinion.value *= CalculateResult(skill);
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }

    public float CalculateResult(float skill) {
        // You always get the same publicity,
        // but the opinion resulting from the Promo varies
        // depending on the skill put into it.
        float successFactor = SuccessProbability(skill);

        // If successful,
        // you get positive public opinion.
        // You also get a random bonus related to the skill involved.
        if (Random.value <= successFactor) {
            return Random.Range(1, 1 + successFactor);

        // If the promo fails, you get some negative public opinion.
        } else {
            return successFactor - 1;
        }
    }

    private float SuccessProbability(float skill) {
        float x = skill * 1/difficulty;

        // This is a function with an asymptote at y=1,
        // so the success probability can never be 1, but it can get very close.
        return 1 - (1/(x+1));
    }
}
