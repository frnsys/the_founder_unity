using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ConsultancyAsset {
    [MenuItem("Assets/Create/Consultancy")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Consultancy>();
    }
}
