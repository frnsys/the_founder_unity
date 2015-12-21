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

        p.primaryFeature = (ProductRecipe.Feature)EditorGUILayout.EnumPopup("Primary Feature", p.primaryFeature);
        p.featureIdeal = EditorGUILayout.FloatField("Ideal Feature Value", p.featureIdeal);
        EditorGUILayout.Space();

        p.maxRevenue = EditorGUILayout.FloatField("Max Revenue", p.maxRevenue);
        EditorGUILayout.Space();

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
