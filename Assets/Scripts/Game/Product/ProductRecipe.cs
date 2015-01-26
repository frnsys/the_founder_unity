/*
 * A product recipe describes how the performance of a given combination
 * of product aspects is influenced by that product's features.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductRecipe : SharedResource<ProductRecipe> {
    // Naming convention:
    // ProductType.ProductType.<etc>.asset

    public List<ProductType> productTypes;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

    // Comma-delimited list of names.
    public string names;

    // Weights: how important a given feature is to the product's performance.
    public float design_W = 1;
    public float marketing_W = 1;
    public float engineering_W = 1;

    // Ideals: some minimum value a feature must achieve for good performance.
    public float design_I = 100;
    public float marketing_I = 100;
    public float engineering_I = 100;

    // How long this product will be profitable on the market.
    public float maxLongevity = 1000;

    // The maximum amount of revenue this product can generate.
    public float maxRevenue = 1000;

    // Bonus effects.
    public EffectSet effects = new EffectSet();

    public override string ToString() {
        return string.Join(".", productTypes.Select(pt => pt.name).ToArray());
    }

    public static ProductRecipe Load(string name) {
        return Resources.Load("Products/Recipes/" + name) as ProductRecipe;
    }

    public static ProductRecipe LoadFromTypes(List<ProductType> productTypes) {
        string name = string.Join(".", productTypes.Select(pt => pt.name).ToArray());
        name = name.Replace("(Clone)", "");
        return Load(name);
    }

    public static ProductRecipe LoadDefault() {
        return Load("Default");
    }

    public static List<ProductRecipe> LoadAll() {
        return new List<ProductRecipe>(Resources.LoadAll<ProductRecipe>("Products/Recipes"));
    }
}
