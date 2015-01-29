/*
 * Any item which can be purchased from the market.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item : TemplateResource<Item> {
    public float cost = 1000;
    public float duration = 0;
    public string description;
    public Store store;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

    public EffectSet effects = new EffectSet();

    // Specify names for tiers 2 and greater, delimited by "|".
    public string upgradeNames;
    public Tier tier;

    public string NameForTier(Tier t) {
        switch (t) {
            case Tier.None:
                return name;
                break;
            case Tier.Cheap:
                return name;
                break;
            case Tier.Good:
                return upgradeNames.Split('|')[0];
            case Tier.Best:
                return upgradeNames.Split('|')[1];
            default:
                return name;
        }
    }

    // Some items can be upgraded.
    // Tier.None items can't be upgraded.
    public enum Tier {
        None,
        Cheap,
        Good,
        Best
    }
}


/*
 * A Store holds specific types of "items".
 * E.g. there's a store for Equipment, Perks,
 * Policies, and Companies.
 */
public enum Store {
    Equipment,
    Perks,
    Policies,
    Companies
}

