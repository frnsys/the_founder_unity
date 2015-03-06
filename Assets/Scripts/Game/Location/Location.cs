using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : SharedResource<Location> {
    public string description;

    // Rotation on the earth.
    public Vector3 rotation;
    public float cost = 1000000;
    public MarketManager.Market market = MarketManager.Market.NorthAmerica;

    public Infrastructure capacity = new Infrastructure();
    public EffectSet effects = new EffectSet();

    public static Location Load(string name) {
        return Resources.Load<Location>("Locations/" + name);
    }

    public static Location[] LoadAll() {
        return Resources.LoadAll<Location>("Locations");
    }
}
