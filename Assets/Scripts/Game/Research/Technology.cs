using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Technology : SharedResource<Technology> {
    public string description;
    public Texture icon;

    public int cost = 1000;

    public EffectSet effects = new EffectSet();

    public Vertical requiredVertical;
    public List<Technology> requiredTechnologies;

    public static new Technology Load(string name) {
        return Resources.Load("Technologies/" + name) as Technology;
    }

    public static List<Technology> LoadAll() {
        return new List<Technology>(Resources.LoadAll<Technology>("Technologies"));
    }

    public bool isAvailable(Company company) {
        if (company.technologies.Contains(this))
            return false;

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
