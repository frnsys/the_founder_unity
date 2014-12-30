/*
 * A set of things which are unlocked together.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {
    public PrereqSet prereqs;

    public List<ProductType> productTypes = new List<ProductType>();
    public List<Worker> workers = new List<Worker>();
    public List<GameEvent> events = new List<GameEvent>();
    public List<Item> items = new List<Item>();
    public List<Store> stores = new List<Store>();
    public List<Location> locations = new List<Location>();
    public List<Vertical> verticals = new List<Vertical>();
    public List<Character> characters = new List<Character>();

    // This "unlocks" an UnlockSet by
    // adding the input UnlockSet's values
    // to this one.
    public void Unlock(UnlockSet us) {
        productTypes.AddRange(us.productTypes);
        workers.AddRange(us.workers);
        events.AddRange(us.events);
        items.AddRange(us.items);
        stores.AddRange(us.stores);
        locations.AddRange(us.locations);
        verticals.AddRange(us.verticals);
        characters.AddRange(us.characters);
    }

    // Apply a prereq set and see if it unlocks the set.
    public bool SatisfyPrereqs(PrereqSet ps) {
        return prereqs.Satisfy(ps);
    }
}
