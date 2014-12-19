/*
 * A set of prequisites which results in some event.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class PrereqSet {
    // Prerequisites
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Vertical> verticals = new List<Vertical>();
    public List<Worker> workers = new List<Worker>();
    public List<GameEvent> events = new List<GameEvent>();
    public List<Item> items = new List<Item>();
    public List<Store> stores = new List<Store>();
    public int researchPoints = 0;

    // This satisfies prereqs by removing the completed ones.
    // Returns true if the prereq set is completed.
    public bool Satisfy(PrereqSet ps) {
        productTypes = productTypes.Except(ps.productTypes).ToList();
        verticals = verticals.Except(ps.verticals).ToList();
        workers = workers.Except(ps.workers).ToList();
        events = events.Except(ps.events).ToList();
        items = items.Except(ps.items).ToList();
        stores = stores.Except(ps.stores).ToList();
        researchPoints -= ps.researchPoints;

        return Completed();
    }

    public bool Completed() {
        if (researchPoints > 0)
            return false;
        else if (productTypes.Count > 0)
            return false;
        else if (verticals.Count > 0)
            return false;
        else if (workers.Count > 0)
            return false;
        else if (events.Count > 0)
            return false;
        else if (items.Count > 0)
            return false;
        else if (stores.Count > 0)
            return false;
        else
            return true;
    }
}
