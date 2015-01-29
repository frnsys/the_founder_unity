using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class PerkAsset {
    [MenuItem("Assets/Create/Perk")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Perk>();
    }
}
