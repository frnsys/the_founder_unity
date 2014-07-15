using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ProductTypeAsset {
    [MenuItem("Assets/Create/ProductType")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<ProductType>();
    }
}
