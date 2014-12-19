using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(ProductRecipe))]
internal class ProductRecipeInspector : Editor {

    ProductRecipe p;

    List<ProductType> productTypes;
    string[] pt_o;

    void OnEnable() {
        p = target as ProductRecipe;

        productTypes = ProductType.LoadAll();
        pt_o = productTypes.Select(i => i.ToString()).ToArray();

        // Defaults
        if (p.productTypes == null)
            p.productTypes = new List<ProductType>();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("ProductTypes");
        EditorGUI.indentLevel = 1;
        for (int i=0; i < p.productTypes.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            int pt_i = EditorGUILayout.Popup(Array.IndexOf(pt_o, p.productTypes[i].ToString()), pt_o);
            p.productTypes[i] = productTypes[pt_i];
            if (GUILayout.Button("Delete")) {
                p.productTypes.Remove(p.productTypes[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Maximum two product types for a product.
        if (p.productTypes.Count <= 2) {
            if (GUILayout.Button("Add New Product Type")) {
                p.productTypes.Add(productTypes[0]);
            }
        }
        EditorGUI.indentLevel = 0;

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

        EditorGUILayout.LabelField("Effects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUIContent.none);

        // Let Unity know to save on changes.
        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != p.ToString()) {
                AssetDatabase.RenameAsset(path, p.ToString());
                AssetDatabase.Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}
