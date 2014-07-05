using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductType : ProductAspect {
    public static ProductType Load(string name) {
        return Resources.Load("Products/Types/" + name) as ProductType;
    }

    public static List<ProductType> LoadAll() {
        return new List<ProductType>(Resources.LoadAll<ProductType>("Products/Types"));
    }
}
