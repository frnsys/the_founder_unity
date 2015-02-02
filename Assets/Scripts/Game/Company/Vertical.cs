using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vertical : SharedResource<Vertical> {
    public float cost = 10000000;
    public string description;

    public Texture texture;
    public Mesh mesh;

    public bool isAvailable(Company company) {
        if (company.verticals.Count < company.locations.Count)
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
