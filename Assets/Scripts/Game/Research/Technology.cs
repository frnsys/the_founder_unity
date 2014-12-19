using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Technology : Resource<Technology>, IHasPrereqs {
    public string description;

    public float requiredResearch = 1000;

    public EffectSet effects = new EffectSet();

    public Vertical requiredVertical;
    public List<Technology> requiredTechnologies;

    public virtual void Awake() {
        // Default values.
        requiredTechnologies = new List<Technology>();
    }

    public static Technology Load(string name) {
        return Resources.Load("Technologies/" + name) as Technology;
    }

    public static List<Technology> LoadAll() {
        return new List<Technology>(Resources.LoadAll<Technology>("Technologies"));
    }

    public bool isAvailable(Company company) {
        // The technology's vertical must be active on the company.
        if (!company.verticals.Contains(requiredVertical))
            return false;

        // The technology's prerequisite technologies must have been researched.
        foreach (Technology t in requiredTechnologies) {
            if (!company.technologies.Contains(t))
                return false;
        }

        return true;
    }
}
