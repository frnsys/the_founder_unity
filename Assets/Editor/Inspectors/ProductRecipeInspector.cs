using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(ProductRecipe))]
internal class ProductRecipeInspector : Editor {

    ProductRecipe p;

    void OnEnable() {
        p = target as ProductRecipe;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("productTypes"), true);

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Names (comma-delimited)");
        p.names = EditorGUILayout.TextArea(p.names);

        EditorGUILayout.LabelField("Description");
        p.description = EditorGUILayout.TextArea(p.description, GUILayout.Height(50));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("synergies"), true);

        p.design_W = EditorGUILayout.FloatField("Design Weight", p.design_W);
        p.marketing_W = EditorGUILayout.FloatField("Marketing Weight", p.marketing_W);
        p.engineering_W = EditorGUILayout.FloatField("Engineering Weight", p.engineering_W);
        EditorGUILayout.Space();

        p.design_I = EditorGUILayout.FloatField("Design Ideal", p.design_I);
        p.marketing_I = EditorGUILayout.FloatField("Marketing Ideal", p.marketing_I);
        p.engineering_I = EditorGUILayout.FloatField("Engineering Ideal", p.engineering_I);
        EditorGUILayout.Space();

        p.maxLongevity = EditorGUILayout.FloatField("Max Longevity (14/week)", p.maxLongevity);
        p.maxRevenue = EditorGUILayout.FloatField("Max Revenue", p.maxRevenue);
        EditorGUILayout.Space();

        if (p.revenueModel == null)
            p.revenueModel = new AnimationCurve();
        p.revenueModel = EditorGUILayout.CurveField("Revenue Model", p.revenueModel, GUILayout.Height(50));

        EditorGUILayout.LabelField("Required Techs");
        for (int i=0; i < p.requiredTechnologies.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            p.requiredTechnologies[i] = (Technology)EditorGUILayout.ObjectField(p.requiredTechnologies[i], typeof(Technology), false);
            if (GUILayout.Button("Delete")) {
                p.requiredTechnologies.Remove(p.requiredTechnologies[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Required Technology")) {
            p.requiredTechnologies.Add(null);
        }

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
