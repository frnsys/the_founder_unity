using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class FounderAsset {
    [MenuItem("Assets/Create/Founder")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Founder>();
    }
}
