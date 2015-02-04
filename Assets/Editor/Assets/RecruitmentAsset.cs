using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class RecruitmentAsset {
    [MenuItem("Assets/Create/Recruitment")]
    public static void CreateAsset() {
        CustomAssetUtility.CreateAsset<Recruitment>();
    }
}
