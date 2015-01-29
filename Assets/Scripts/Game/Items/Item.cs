/*
 * Any item which can be purchased from the market.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item : TemplateResource<Item> {
    public float cost = 1000;
    public float duration = 0; // TO DO delete this
    public string description;
    public Store store;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

    public EffectSet effects = new EffectSet();
}


/*
 * A Store holds specific types of "items".
 */
public enum Store {
    Policies,
    Companies
}

