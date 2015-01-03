using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Location : ScriptableObject {
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
}
