using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ProductRecipeAsset {
    [MenuItem("Assets/Create/ProductRecipe")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<ProductRecipe>();
    }
}
