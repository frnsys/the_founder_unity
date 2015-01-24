/*
 * A bundle of different effects that affect products.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ProductEffect : IEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Vertical> verticals = new List<Vertical>();
    public StatBuff buff;

    public override void Apply(Company company) {
        List<Product> matchingProducts = company.FindMatchingProducts(productTypes);
        foreach (Product product in matchingProducts) {
            if (!product.developing)
                product.ApplyBuff(buff);
        }
    }

    public override void Remove(Company company) {
        List<Product> matchingProducts = company.FindMatchingProducts(productTypes);
        foreach (Product product in matchingProducts) {
            if (!product.developing)
                product.RemoveBuff(buff);
        }
    }

    public void Apply(Product product) {
        if (IsEligibleForEffect(product))
            product.ApplyBuff(buff);
    }

    public void Remove(Product product) {
        if (IsEligibleForEffect(product))
            product.RemoveBuff(buff);
    }

    private bool IsEligibleForEffect(Product p) {
        // If the product effect is indiscriminate (i.e. doesn't specify any product types or verticals), it applies to every product.
        // Otherwise, a product must contain at least one of the specified effect's product types or verticals.
        if (verticals.Count == 0 && productTypes.Count == 0)
            return true;
        else if (verticals.Count == 0 && productTypes.Intersect(p.productTypes).Any())
            return true;
        else if (productTypes.Count == 0 && verticals.Intersect(p.requiredVerticals).Any())
            return true;
        else if (productTypes.Intersect(p.productTypes).Any() && verticals.Intersect(p.requiredVerticals).Any())
            return true;
        else
            return false;
    }
}
