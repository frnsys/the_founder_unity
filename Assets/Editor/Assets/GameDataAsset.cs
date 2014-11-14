using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GameDataAsset {
    [MenuItem("Assets/Create/GameData")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<GameData>();
    }
}
