using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpecialProject : TemplateResource<SpecialProject> {
    public string description;
    public float cost;
    public float requiredProgress;
    public EffectSet effects = new EffectSet();
    public Infrastructure requiredInfrastructure;

    public Mesh mesh;
    public Texture texture;

    public override string ToString() {
        return name;
    }

    [SerializeField, HideInInspector]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    static public event System.Action<SpecialProject> Completed;
    public bool Develop(float amount) {
        _progress += amount;
        if (progress >= 1) {
            if (Completed != null)
                Completed(this);
            return true;
        }
        return false;
    }

    public bool isAvailable(Company company) {
        // Check that the required infrastructure is available.
        return company.cash.value >= cost && company.availableInfrastructure >= requiredInfrastructure;
    }
}


