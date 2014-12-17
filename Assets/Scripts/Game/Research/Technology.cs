using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Technology : ScriptableObject, IHasPrereqs {
    public string description;

    public Research requiredResearch = new Research(100,100,100);

    public EffectSet effects = new EffectSet();

    public Vertical requiredVertical;
    public List<Technology> requiredTechnologies;

    public bool isAvailable(Company company) {
        if (!company.verticals.Contains(requiredVertical))
            return false;

        foreach (Technology t in requiredTechnologies) {
            if (!company.technologies.Contains(t))
                return false;
        }

        return true;
    }
}
