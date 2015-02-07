using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Recruitment : TemplateResource<Recruitment> {
    public Texture icon;
    public float cost;
    public float targetScore;
    private float requiredProgress;

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    void Awake() {
        requiredProgress = 100;
    }

    static public event System.Action<Recruitment> Completed;
    public bool Develop() {
        // TESTING. This should be a lower value.
        _progress += 50;
        Debug.Log(progress);
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }
}
