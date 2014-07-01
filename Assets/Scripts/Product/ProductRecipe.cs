using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductRecipe : ScriptableObject {
    // Naming convention for these assets is:
    // ProductType.Industry.Market.asset

    public ProductType productType = ProductType.Social_Network;
    public Industry industry = Industry.Space;
    public Market market = Market.Millenials;

    // Weights
    public float appeal_W = 1;
    public float usability_W = 1;
    public float performance_W = 1;

    // Ideals
    public float appeal_I = 100;
    public float usability_I = 100;
    public float performance_I = 100;

    public float progressRequired = 1000;

    public float maxLongevity = 1000;
    public float maxRevenue = 1000;
    public float maintenance = 1000;

    public List<string> outcomes = new List<string>();

    public string ToString() {
        return productType.ToString() + "." + industry.ToString() + "." + market.ToString();
    }
}
