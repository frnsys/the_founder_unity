/*
 * A bundle of different effects that affect products.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ProductEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Vertical> verticals = new List<Vertical>();
    public StatBuff buff;

    public ProductEffect() {
        buff = new StatBuff("Design", 0);
    }

    public ProductEffect(string name) {
        buff = new StatBuff(name, 0);
    }

    public ProductEffect(ProductEffect pe) {
        buff = new StatBuff(pe.buff.name, pe.buff.value);
        productTypes = pe.productTypes;
        verticals = pe.verticals;
    }

    public void Apply(Company company) {
        // TODO update
        //List<Product> matchingProducts = company.FindMatchingProducts(productTypes);
        //foreach (Product product in matchingProducts) {
            //if (!product.developing)
                //product.ApplyBuff(buff);
        //}
    }

    public void Remove(Company company) {
        // TODO update
        //List<Product> matchingProducts = company.FindMatchingProducts(productTypes);
        //foreach (Product product in matchingProducts) {
            //if (!product.developing)
                //product.RemoveBuff(buff);
        //}
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

    public bool Equals(ProductEffect pe) {
        if (productTypes.Count != pe.productTypes.Count)
            return false;

        if (verticals.Count != pe.verticals.Count)
            return false;

        if (!buff.Equals(pe.buff))
            return false;

        for (int i=0; i<productTypes.Count; i++) {
            if (productTypes[i] != pe.productTypes[i])
                return false;
        }

        for (int i=0; i<verticals.Count; i++) {
            if (verticals[i] != pe.verticals[i])
                return false;
        }

        return true;
    }
}
