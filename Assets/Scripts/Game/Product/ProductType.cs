using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductType : SharedResource<ProductType> {
    // For the physical representation of the product.
    public Mesh mesh;

    public override string ToString() {
        return name;
    }

    public static new ProductType Load(string name) {
        return Resources.Load("Products/Types/" + name) as ProductType;
    }

    public static List<ProductType> LoadAll() {
        return new List<ProductType>(Resources.LoadAll<ProductType>("Products/Types"));
    }

    public List<Vertical> requiredVerticals = new List<Vertical>();
    public bool isAvailable(Company company) {
        if (!GameManager.Instance.unlocked.productTypes.Contains(this))
            return false;

        // Check that all required verticals are active on the company.
        return !requiredVerticals.Except(company.verticals).Any();
    }
}
