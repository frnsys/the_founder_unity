using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class MarketAsset {
    [MenuItem("Assets/Create/Market")]
    public static void CreateAsset() {
        Market asset = CustomAssetUtility.CreateAsset<Market>();
    }
}
