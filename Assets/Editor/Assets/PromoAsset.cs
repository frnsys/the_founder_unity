using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class PromoAsset {
    [MenuItem("Assets/Create/Promo")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Promo>();
    }
}
