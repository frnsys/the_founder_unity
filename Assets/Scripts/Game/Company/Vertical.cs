using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vertical : SharedResource<Vertical> {
    public float cost = 10000000;
    public string description;
    public Mesh mesh;

    public static new Vertical Load(string name) {
        return Resources.Load<Vertical>("Verticals/" + name);
    }

    public static List<Vertical> LoadAll() {
        return new List<Vertical>(Resources.LoadAll<Vertical>("Verticals/"));
    }
}
