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
    public enum Feature {
        Design,
        Engineering,
        Marketing
    }

    // Comma-delimited list of names.
    public string names;
    public string description;

    // "Synergistic" product recipes.
    public ProductRecipe[] synergies;

    // Weights: how important a given feature is to the product's performance.
    public Feature primaryFeature;

    // Ideals: some minimum value a feature must achieve for good performance.
    public float featureIdeal = 10;

    // How long this product will be profitable on the market.
    // This is used to modify the revenue model curve such that
    // it is from 0 to maxLongevity along the time (x) axis.
    public float maxLongevity = 1000;

    // The maximum amount of revenue this product can generate.
    // This is used to modify the revenue model curve such that
    // it's peak (y=1.0) along the y axis is at maxRevenue.
    public float maxRevenue = 1000;
    public AnimationCurve revenueModel;

    public ProductType[] productTypes;

    // Bonus effects.
    public EffectSet effects = new EffectSet();

    // Required technologies for the recipe.
    // The recipe can still be built without the required techs,
    // but it will perform with a penalty.
    public List<Technology> requiredTechnologies;

    public override string ToString() {
        return string.Join(".", productTypes.Where(pt => pt != null).OrderBy(pt => pt.name).Select(pt => pt.name).ToArray());
    }

    public static new ProductRecipe Load(string name) {
        return Resources.Load("Products/Recipes/" + name) as ProductRecipe;
    }

    public static ProductRecipe LoadFromTypes(List<ProductType> productTypes) {
        string name = string.Join(".", productTypes.OrderBy(pt => pt.name).Select(pt => pt.name).ToArray());
        name = name.Replace("(Clone)", "");
        return Load(name);
    }

    public static ProductRecipe LoadDefault() {
        return Load("Default");
    }

    public static List<ProductRecipe> LoadAll() {
        return new List<ProductRecipe>(Resources.LoadAll<ProductRecipe>("Products/Recipes"));
    }

    // A generic name just based on the product types.
    public string genericName {
        get {
            return string.Join(" + ", productTypes.Select(pt => pt.name).ToArray());
        }
    }
}
