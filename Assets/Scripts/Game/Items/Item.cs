using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : ScriptableObject, IUnlockable {
    public float cost = 1000;
    public float duration = 0;
    public string description;
    public Store store;

    public List<StatBuff> productBuffs = new List<StatBuff>();
    public List<StatBuff> workerBuffs = new List<StatBuff>();
    public List<Industry> industries = new List<Industry>();
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Market> markets = new List<Market>();

    public Item(string name_, float cost_, List<Industry> industries_,
        List<ProductType> productTypes_, List<Market> markets_
    ) {
        name = name_;
        cost = cost_;
        industries = industries_;
        productTypes = productTypes_;
        markets = markets_;
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
