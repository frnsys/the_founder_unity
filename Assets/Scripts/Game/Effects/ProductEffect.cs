/*
 * A bundle of different effects that affect products.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductEffect : IEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Vertical> verticals = new List<Vertical>();
    public StatBuff buff;

    public void Apply(Company company) {
        // to do
    }

    public void Remove(Company company) {
        // to do
    }
}
