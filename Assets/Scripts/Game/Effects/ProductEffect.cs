/*
 * A bundle of different effects that affect products.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ProductEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Vertical> verticals = new List<Vertical>();
    public StatBuff buff;
}
