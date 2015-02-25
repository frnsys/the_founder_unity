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
    // Comma-delimited list of names.
    public string names;
    public string description;

    public ProductType[] productTypes;
    public AnimationCurve revenueModel;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

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
}
