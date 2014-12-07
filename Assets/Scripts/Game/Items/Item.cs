/*
 * Any item which can be purchased from the market.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item : ScriptableObject {
    public float cost = 1000;
    public float duration = 0;
    public string description;
    public Store store;

    public EffectSet effects = new EffectSet();

    public Item(string name_, float cost_) {
        name = name_;
        cost = cost_;
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
