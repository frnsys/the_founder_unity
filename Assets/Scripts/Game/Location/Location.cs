using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : TemplateResource<Location> {
    public string description;

    public float cost = 1000000;
    public MarketManager.Market market = MarketManager.Market.NorthAmerica;

    public Infrastructure capacity = new Infrastructure();
    public Infrastructure infrastructure = new Infrastructure();
    public EffectSet effects = new EffectSet();

    public Infrastructure availableInfrastructureCapacity {
        get { return capacity - infrastructure; }
    }

    public bool HasCapacityFor(Infrastructure i) {
        return availableInfrastructureCapacity >= i;
    }

    public Vector3 rotation;

    public static Location Load(string name) {
        Location loc = Resources.Load<Location>("Locations/" + name) as Location;
        return loc.Clone();
    }
}
