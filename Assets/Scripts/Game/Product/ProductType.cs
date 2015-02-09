using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductType : SharedResource<ProductType> {
    // Comma-delimited list of names.
    public string names;
    public string description;

    // The difficulty modifier for this product type.
    // This is for calculating the amount of progress required to develop the product.
    public float difficulty;

    // Weights: how important a given feature is to the product's performance.
    public float design_W = 1;
    public float marketing_W = 1;
    public float engineering_W = 1;

    // Ideals: some minimum value a feature must achieve for good performance.
    public float design_I = 100;
    public float marketing_I = 100;
    public float engineering_I = 100;

    // How long this product will be profitable on the market.
    // This is used to modify the revenue model curve such that
    // it is from 0 to maxLongevity along the time (x) axis.
    public float maxLongevity = 1000;

    // The maximum amount of revenue this product can generate.
    // This is used to modify the revenue model curve such that
    // it's peak (y=1.0) along the y axis is at maxRevenue.
    public float maxRevenue = 1000;
    public AnimationCurve revenueModel;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

    // Bonus effects.
    public EffectSet effects = new EffectSet();

    public override string ToString() {
        return name;
    }

    public static new ProductType Load(string name) {
        return Resources.Load("Products/Types/" + name) as ProductType;
    }

    public static List<ProductType> LoadAll() {
        return new List<ProductType>(Resources.LoadAll<ProductType>("Products/Types"));
    }

    // Note: we don't have required technologies because a technology is necessary
    // for *unlocking* a product. Technologies don't disappear so it never needs to be checked again after the product is unlocked.
    public List<Vertical> requiredVerticals;
    public Infrastructure requiredInfrastructure;
    public bool isAvailable(Company company) {
        // Check that all required verticals are active on the company.
        if (requiredVerticals.Except(company.verticals).Any())
            return false;

        // Check that the required infrastructure is available.
        return company.availableInfrastructure >= requiredInfrastructure;
    }

    // Generalize a "points" metric which is just the total number of infrastructure required.
    public int points {
        get {
            int total = 0;
            foreach(KeyValuePair<Infrastructure.Type, int> i in requiredInfrastructure) {
                total += i.Value;
            }
            return total;
        }
    }
}
