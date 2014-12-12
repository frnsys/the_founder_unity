using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GameConfigAsset {
    [MenuItem("Assets/Create/GameConfig")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<GameConfig>();
    }
}
