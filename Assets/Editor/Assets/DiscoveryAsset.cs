using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class DiscoveryAsset {
    [MenuItem("Assets/Create/Discovery")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Discovery>();
    }
}
