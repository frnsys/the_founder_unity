using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductType : SharedResource<ProductType> {
    public string description;

    // The difficulty modifier for this product type.
    public float difficulty;

    // For the physical representation of the product.
    public Mesh mesh;
    public Texture texture;

    public override string ToString() {
        return name;
    }

    public static ProductType Load(string name) {
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
