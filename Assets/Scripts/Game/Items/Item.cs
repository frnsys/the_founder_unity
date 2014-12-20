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
