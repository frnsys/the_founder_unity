/*
 * A set of things which are unlocked together.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Worker> workers = new List<Worker>();
    public List<Perk> perks = new List<Perk>();
    public List<Location> locations = new List<Location>();
    public List<Vertical> verticals = new List<Vertical>();
    public List<Promo> promos = new List<Promo>();
    public List<Recruitment> recruitments = new List<Recruitment>();
    public List<MiniCompany> companies = new List<MiniCompany>();

    // This "unlocks" an UnlockSet by
    // adding the input UnlockSet's values
    // to this one.
    public void Unlock(UnlockSet us) {
        productTypes.AddRange(us.productTypes);
        workers.AddRange(us.workers);
        perks.AddRange(us.perks);
        locations.AddRange(us.locations);
        verticals.AddRange(us.verticals);
        promos.AddRange(us.promos);
        recruitments.AddRange(us.recruitments);
        companies.AddRange(us.companies);
    }
}
