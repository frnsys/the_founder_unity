using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class AICompanyAsset {
    [MenuItem("Assets/Create/AICompany")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<AICompany>();
    }
}
