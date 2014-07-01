using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;

[InitializeOnLoad]
[CustomEditor(typeof(ProductRecipe))]
internal class ProductRecipeInspector : Editor {

    static ProductRecipeInspector() {
        EditorApplication.update += RunOnce;
    }

    static void RunOnce() {
        EditorApplication.update -= RunOnce;
    }

    public override void OnInspectorGUI() {
        ProductRecipe p = target as ProductRecipe;

        p.productType = (ProductType)EditorGUILayout.EnumPopup("Product Type", p.productType);
        p.industry = (Industry)EditorGUILayout.EnumPopup("Industry", p.industry);
        p.market = (Market)EditorGUILayout.EnumPopup("Market", p.market);
        EditorGUILayout.Space();

        p.appeal_W = EditorGUILayout.FloatField("Appeal Weight", p.appeal_W);
        p.usability_W = EditorGUILayout.FloatField("Usability Weight", p.usability_W);
        p.performance_W = EditorGUILayout.FloatField("Performance Weight", p.performance_W);
        EditorGUILayout.Space();

        p.appeal_I = EditorGUILayout.FloatField("Appeal Ideal", p.appeal_I);
        p.usability_I = EditorGUILayout.FloatField("Usability Ideal", p.usability_I);
        p.performance_I = EditorGUILayout.FloatField("Performance Ideal", p.performance_I);
        EditorGUILayout.Space();

        p.progressRequired = EditorGUILayout.FloatField("Progress Required", p.progressRequired);
        p.maxLongevity = EditorGUILayout.FloatField("Max Longevity", p.maxLongevity);
        p.maxRevenue = EditorGUILayout.FloatField("Max Revenue", p.maxRevenue);
        p.maintenance = EditorGUILayout.FloatField("Maintenance Cost", p.maintenance);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Outcomes");
        EditorGUI.indentLevel = 1;
        for (int i=0; i < p.outcomes.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            p.outcomes[i] = EditorGUILayout.TextField(p.outcomes[i]);
            if (GUILayout.Button("Delete")) {
                p.outcomes.Remove(p.outcomes[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Outcome")) {
            p.outcomes.Add("New outcome");
        }
        EditorGUI.indentLevel = 0;

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != p.ToString()) {
                AssetDatabase.RenameAsset(path, p.ToString());
                AssetDatabase.Refresh();
            }
        }
    }

}
