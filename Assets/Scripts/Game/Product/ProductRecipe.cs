using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductRecipe : ScriptableObject {
    // Naming convention for these assets is:
    // ProductType.Industry.Market.asset

    public ProductType productType;
    public Industry industry;
    public Market market;

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

    public override string ToString() {
        return productType.ToString() + "." + industry.ToString() + "." + market.ToString();
    }

    public static ProductRecipe Load(ProductType pt, Industry i, Market m) {
        return Resources.Load("Products/Recipes/" + pt.ToString() + "." + i.ToString() + "." + m.ToString()) as ProductRecipe;
    }

    public static ProductRecipe Load() {
        return Resources.Load("Products/Recipes/Default") as ProductRecipe;
    }
}
