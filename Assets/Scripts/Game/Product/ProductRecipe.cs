/*
 * A product recipe describes how the performance of a given combination
 * of product aspects is influenced by that product's features.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductRecipe : ScriptableObject {
    // Naming convention for these assets is:
    // ProductType.Industry.Market.asset

    public ProductType productType;
    public Industry industry;
    public Market market;

    // Weights: how important a given feature is to the product's performance.
    public float appeal_W = 1;
    public float usability_W = 1;
    public float performance_W = 1;

    // Ideals: some minimum value a feature must achieve for good performance.
    public float appeal_I = 100;
    public float usability_I = 100;
    public float performance_I = 100;

    // The amount of progress necessary for finishing the product.
    public float progressRequired = 1000;

    // How long this product will be profitable on the market.
    public float maxLongevity = 1000;

    // The maximum amount of revenue this product can generate.
    public float maxRevenue = 1000;

    // The cost of this product's maintenance.
    public float maintenance = 1000;


    // TO DO use this or maybe it's not being used?
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
