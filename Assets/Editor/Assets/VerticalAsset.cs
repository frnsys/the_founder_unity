using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class VerticalAsset {
    [MenuItem("Assets/Create/Vertical")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Vertical>();
    }
}
