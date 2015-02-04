using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : TemplateResource<Promo> {
    public OpinionEvent opinionEvent;
    public Texture icon;
    public string description;
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
    public OpinionEvent Develop(float amount, float skill) {
        _progress += amount;
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return CalculateResult(skill);
        }
        return null;
    }

    public OpinionEvent CalculateResult(float skill) {
        GameEvent ev = GameEvent.LoadSpecialEvent("Promo Success");
        float pp = skill/difficulty;
        float majorSuccessProb = Mathf.Max(0, pp - 1);
        float roll = Random.value;
        float num_people = 1000 * opinionEvent.publicity.value;
        OpinionEvent result = new OpinionEvent(
                opinionEvent.opinion.value,
                opinionEvent.publicity.value);

        // A major success has bonuses.
        if (roll < majorSuccessProb) {
            // Even higher skill levels yield extra bonuses.
            float multiplier = 1.2f + Mathf.Max(0, pp - 2)/10;
            result.opinion.value *= multiplier;
            result.publicity.value *= multiplier;

            ev = GameEvent.LoadSpecialEvent("Promo Major Success");
            num_people = 10000 * result.publicity.value;

        // Failure
        } else if (roll > pp) {
            result.opinion.value *= 0;
            result.publicity.value *= 0.1f;
            ev = GameEvent.LoadSpecialEvent("Promo Failure");
            num_people = 1000 * result.publicity.value;
        }

        ev.description = ev.description.Replace("<NUM_PEOPLE>", string.Format("{0:n}", num_people));
        GameEvent.Trigger(ev);
        return result;
    }
}
