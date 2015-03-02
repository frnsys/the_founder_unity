using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : TemplateResource<Promo> {
    public Texture icon;
    public string description;
    public float cost;
    public float requiredProgress;
    public GameObject level;

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        // Base promo time is 4 weeks at 12 cycles/week
        get { return _progress/requiredProgress * 48f; }
    }

    static public event System.Action<Promo> Completed;
    public bool Develop(float amount, float skill) {
        _progress += amount;
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }
}
