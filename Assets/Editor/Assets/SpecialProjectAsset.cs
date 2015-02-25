using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SpecialProjectAsset {
    [MenuItem("Assets/Create/SpecialProject")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<SpecialProject>();
    }
}
