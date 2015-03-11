/*
 * A set of things which are unlocked together.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class UnlockSet {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Perk> perks = new List<Perk>();
    public List<Location> locations = new List<Location>();
    public List<Vertical> verticals = new List<Vertical>();
    public List<MiniCompany> companies = new List<MiniCompany>();
    public List<SpecialProject> specialProjects = new List<SpecialProject>();

    static public event System.Action<UnlockSet> Unlocked;

    // This "unlocks" an UnlockSet by
    // adding the input UnlockSet's values
    // to this one.
    public void Unlock(UnlockSet us) {
        foreach (ProductType o in us.productTypes) {
            if (!Duplicate(o, productTypes))
                productTypes.Add(o);
        }
        foreach (Perk o in us.perks) {
            if (!Duplicate(o, perks))
                perks.Add(o);
        }
        foreach (Location o in us.locations) {
            if (!Duplicate(o, locations))
                locations.Add(o);
        }
        foreach (Vertical o in us.verticals) {
            if (!Duplicate(o, verticals))
                verticals.Add(o);
        }
        foreach (MiniCompany o in us.companies) {
            if (!Duplicate(o, companies))
                companies.Add(o);
        }
        foreach (SpecialProject o in us.specialProjects) {
            if (!Duplicate(o, specialProjects))
                specialProjects.Add(o);
        }

        if (Unlocked != null)
            Unlocked(us);
    }

    private bool Duplicate<T>(T obj, List<T> objs) where T : Object {
        return objs.FirstOrDefault(o => o.name == obj.name) != null;
    }
}
