/*
 * Any item which can be purchased from the market.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Perk : SharedResource<Perk> {
    [System.Serializable]
    public class Upgrade {
        public string name;
        public float cost = 1000;
        public string description;
        public Office.Type requiredOffice;

        // For the physical representation of the product.
        public Mesh mesh;

        public EffectSet effects = new EffectSet();
        public List<Technology> requiredTechnologies = new List<Technology>();

        public bool Available(Company c) {
            if (c.office < requiredOffice)
                return false;
            foreach (Technology t in requiredTechnologies) {
                if (!c.technologies.Contains(t))
                    return false;
            }
            return true;
        }
    }

    public List<Upgrade> upgrades = new List<Upgrade>();

    public static new Perk Load(string name) {
        return Resources.Load("Perks/" + name) as Perk;
    }
}
