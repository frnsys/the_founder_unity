using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class TechnologyAsset {
    [MenuItem("Assets/Create/Technology")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Technology>();
    }
}
