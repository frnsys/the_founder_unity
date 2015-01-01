using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : ScriptableObject {
    public OpinionEvent opinionEvent;
    public float cost;
    public float requiredProgress;

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    void Awake() {
        opinionEvent = new OpinionEvent(0, 0);
    }

    static public event System.Action<Promo> Completed;
    public bool Develop(float amount) {
        _progress += amount;
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }
}
