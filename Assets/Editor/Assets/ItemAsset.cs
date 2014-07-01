using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ItemAsset {
    [MenuItem("Assets/Create/Item")]
    public static void CreateAsset() {
        Item item = CustomAssetUtility.CreateAsset<Item>();
    }
}
