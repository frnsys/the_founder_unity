using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class IndustryAsset {
    [MenuItem("Assets/Create/Industry")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Industry>();
    }
}
