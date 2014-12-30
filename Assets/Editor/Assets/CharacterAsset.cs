using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class CharacterAsset {
    [MenuItem("Assets/Create/Character")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Character>();
    }
}
