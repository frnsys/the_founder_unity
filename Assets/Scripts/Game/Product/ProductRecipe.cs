/*
 * A product recipe describes how the performance of a given combination
 * of product aspects is influenced by that product's features.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductRecipe : ScriptableObject {
    // Naming convention:
    // ProductType.ProductType.<etc>.asset

    public List<ProductType> productTypes;

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
    // This can eventually become where product bonus effects are kept.
    public List<string> outcomes = new List<string>();

    public override string ToString() {
        return string.Join(".", productTypes.Select(pt => pt.name).ToArray());
    }

    public static ProductRecipe Load(List<ProductType> productTypes) {
        string name = string.Join(".", productTypes.Select(pt => pt.name).ToArray());
        name = name.Replace("(Clone)", "");
        return Instantiate(Resources.Load("Products/Recipes/" + name)) as ProductRecipe;
    }

    public static ProductRecipe Load() {
        return Instantiate(Resources.Load("Products/Recipes/Default")) as ProductRecipe;
    }
}
