using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GameEventAsset {
    [MenuItem("Assets/Create/GameEvent")]
    public static void CreateAsset() {
        GameEvent gameEvent = CustomAssetUtility.CreateAsset<GameEvent>();

        gameEvent.effects = new List<GameEffect>();
    }
}
