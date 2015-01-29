/*
 * A set of things which are unlocked together.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Worker> workers = new List<Worker>();
    public List<Item> items = new List<Item>();
    public List<Perk> perks = new List<Perk>();
    public List<Store> stores = new List<Store>();
    public List<Location> locations = new List<Location>();
    public List<Vertical> verticals = new List<Vertical>();
    public List<Promo> promos = new List<Promo>();

    // This "unlocks" an UnlockSet by
    // adding the input UnlockSet's values
    // to this one.
    public void Unlock(UnlockSet us) {
        productTypes.AddRange(us.productTypes);
        workers.AddRange(us.workers);
        items.AddRange(us.items);
        perks.AddRange(us.perks);
        stores.AddRange(us.stores);
        locations.AddRange(us.locations);
        verticals.AddRange(us.verticals);
        promos.AddRange(us.promos);
    }
}
