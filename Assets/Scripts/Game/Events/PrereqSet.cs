/*
 * A set of prequisites which results in some event.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PrereqSet {
    // Prerequisites
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<Worker> workers = new List<Worker>();
    public List<GameEvent> events = new List<GameEvent>();
    public List<Item> items = new List<Item>();
    public List<Store> stores = new List<Store>();
    public List<Consultancy> consultancies = new List<Consultancy>();
    public int researchPoints = 0;

    // This satisfies prereqs by removing the completed ones.
    // Returns true if the prereq set is completed.
    public bool Satisfy(PrereqSet ps) {
        productTypes = productTypes.Except(ps.productTypes).ToList();
        industries = industries.Except(ps.industries).ToList();
        markets = markets.Except(ps.markets).ToList();
        workers = workers.Except(ps.workers).ToList();
        events = events.Except(ps.events).ToList();
        items = items.Except(ps.items).ToList();
        stores = stores.Except(ps.stores).ToList();
        consultancies = consultancies.Except(ps.consultancies).ToList();
        researchPoints -= ps.researchPoints;

        return Completed();
    }

    public bool Completed() {
        if (researchPoints > 0)
            return false;
        else if (productTypes.Count > 0)
            return false;
        else if (industries.Count > 0)
            return false;
        else if (markets.Count > 0)
            return false;
        else if (workers.Count > 0)
            return false;
        else if (events.Count > 0)
            return false;
        else if (items.Count > 0)
            return false;
        else if (stores.Count > 0)
            return false;
        else if (consultancies.Count > 0)
            return false;
        else
            return true;
    }
}
