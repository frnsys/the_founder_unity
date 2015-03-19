using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class LobbyAsset {
    [MenuItem("Assets/Create/Lobby")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Lobby>();
    }
}
