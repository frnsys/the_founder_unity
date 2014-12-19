using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vertical : Resource<Vertical>, IHasPrereqs {
    public float cost = 10000000;

    public bool isAvailable(Company company) {
        // TO DO this should be the number of locations
        if (company.verticals.Count < company.workers.Count)
            return true;
        return false;
    }

    public static Vertical Load(string name) {
        return Resources.Load("Verticals/" + name) as Vertical;
    }

    public static List<Vertical> LoadAll() {
        return new List<Vertical>(Resources.LoadAll<Vertical>("Verticals/"));
    }
}
