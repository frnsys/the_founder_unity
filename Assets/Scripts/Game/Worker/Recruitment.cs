using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Recruitment : TemplateResource<Recruitment> {
    public Texture icon;
    public float cost;
    public float targetScore;
    public string description;
    private float requiredProgress;

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    void Awake() {
        // At 12cycles/week, this is 4 weeks.
        requiredProgress = 48f;
    }

    static public event System.Action<Recruitment> Completed;
    public bool Develop() {
        _progress += 1;
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }
}
