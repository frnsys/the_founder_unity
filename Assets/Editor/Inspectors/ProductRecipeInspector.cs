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

        p.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", p.mesh, typeof(Mesh), false);
        p.texture = (Texture)EditorGUILayout.ObjectField("Texture", p.texture, typeof(Texture), false);

        if (p.revenueModel == null)
            p.revenueModel = new AnimationCurve();
        p.revenueModel = EditorGUILayout.CurveField("Revenue Model", p.revenueModel, GUILayout.Height(50));

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
