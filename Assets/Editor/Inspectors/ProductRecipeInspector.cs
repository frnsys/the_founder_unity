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

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Names (comma-delimited)");
        p.names = EditorGUILayout.TextArea(p.names);

        EditorGUILayout.LabelField("Description");
        p.description = EditorGUILayout.TextArea(p.description, GUILayout.Height(50));
        EditorGUILayout.Space();

        p.design_W = EditorGUILayout.FloatField("Design Weight", p.design_W);
        p.marketing_W = EditorGUILayout.FloatField("Marketing Weight", p.marketing_W);
        p.engineering_W = EditorGUILayout.FloatField("Engineering Weight", p.engineering_W);
        EditorGUILayout.Space();

        p.design_I = EditorGUILayout.FloatField("Design Ideal", p.design_I);
        p.marketing_I = EditorGUILayout.FloatField("Marketing Ideal", p.marketing_I);
        p.engineering_I = EditorGUILayout.FloatField("Engineering Ideal", p.engineering_I);
        EditorGUILayout.Space();

        p.maxLongevity = EditorGUILayout.FloatField("Max Longevity", p.maxLongevity);
        p.maxRevenue = EditorGUILayout.FloatField("Max Revenue", p.maxRevenue);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Effects");
        if (p.effects == null)
            p.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(p, p.effects);
        EditorGUILayout.Space();

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
