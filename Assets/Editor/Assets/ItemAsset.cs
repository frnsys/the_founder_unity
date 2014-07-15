using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ItemAsset {
    [MenuItem("Assets/Create/Item")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Item>();
    }
}
