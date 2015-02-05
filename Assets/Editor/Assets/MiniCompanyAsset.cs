using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class MiniCompanyAsset {
    [MenuItem("Assets/Create/MiniCompany")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<MiniCompany>();
    }
}
