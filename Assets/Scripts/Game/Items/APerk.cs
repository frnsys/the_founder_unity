/*
 * Any item which can be purchased from the market.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class APerk {
    // The index of the currently-active upgrade.
    public int upgradeLevel = 0;

    [SerializeField]
    private Perk perk;

    public APerk(Perk p) {
        perk = p;
    }

    public List<Perk.Upgrade> upgrades {
        get { return perk.upgrades; }
    }
    public string name {
        get { return perk.name; }
    }
    public Perk.Upgrade current {
        get { return perk.upgrades[upgradeLevel]; }
    }
    public Perk.Upgrade next {
        get { return hasNext ? perk.upgrades[upgradeLevel + 1] : null; }
    }
    public bool hasNext {
        get { return upgradeLevel < perk.upgrades.Count - 1; }
    }
    public bool NextAvailable(Company c) {
        // A perks availability depends on whether or not the company
        // has the necessary technologies and a high enough office level upgrade.
        return !hasNext ? false : next.Available(c);
    }
    public float cost {
        get { return current.cost; }
    }
    public EffectSet effects {
        get { return current.effects; }
    }
    public string description {
        get { return current.description; }
    }
    public Mesh mesh  {
        get { return current.mesh; }
    }
}
