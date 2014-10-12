using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {

    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<Worker> workers = new List<Worker>();
    public List<GameEvent> events = new List<GameEvent>();
    public List<Item> items = new List<Item>();
    public List<Store> stores = new List<Store>();

    // This "unlocks" an UnlockSet by
    // adding the input UnlockSet's values
    // to this one.
    public void Unlock(UnlockSet us) {
        productTypes.AddRange(us.productTypes);
        industries.AddRange(us.industries);
        markets.AddRange(us.markets);
        workers.AddRange(us.workers);
        events.AddRange(us.events);
        items.AddRange(us.items);
        stores.AddRange(us.stores);
    }
}
