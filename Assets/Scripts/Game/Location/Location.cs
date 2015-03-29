using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : SharedResource<Location> {
    // Rotation on the earth.
    public Vector3 rotation;
    public float cost = 1000000;
    public string description;
    public MarketManager.Market market = MarketManager.Market.NorthAmerica;
    public EffectSet effects = new EffectSet();

    public Mesh altMesh;
    public Material altMat;

    public static new Location Load(string name) {
        return Resources.Load<Location>("Locations/" + name);
    }

    public static Location[] LoadAll() {
        return Resources.LoadAll<Location>("Locations");
    }
}
