using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class LocationAsset {
    [MenuItem("Assets/Create/Location")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Location>();
    }
}
