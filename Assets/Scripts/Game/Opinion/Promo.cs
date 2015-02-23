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
        get { return _progress/requiredProgress; }
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
