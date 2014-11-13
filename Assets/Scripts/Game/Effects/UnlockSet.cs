/*
 * A set of things which are unlocked together.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {

    public PrereqSet prereqs;

    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<Worker> workers = new List<Worker>();
    public List<GameEvent> events = new List<GameEvent>();
    public List<Item> items = new List<Item>();
    public List<Store> stores = new List<Store>();
    public List<Consultancy> consultancies = new List<Consultancy>();

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
        consultancies.AddRange(us.consultancies);
    }

    // Apply a prereq set and see if it unlocks the set.
    public bool SatisfyPrereqs(PrereqSet ps) {
        return prereqs.Satisfy(ps);
    }
}