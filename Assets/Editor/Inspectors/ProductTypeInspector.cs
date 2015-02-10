using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(ProductType))]
internal class ProductTypeInspector : Editor {

    ProductType p;

    void OnEnable() {
        p = target as ProductType;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        p.name = EditorGUILayout.TextField("Name", p.name);

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

        p.difficulty = EditorGUILayout.FloatField("Difficulty", p.difficulty);

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

        EditorGUILayout.LabelField("Required Verticals");
        for (int i=0; i < p.requiredVerticals.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            p.requiredVerticals[i] = (Vertical)EditorGUILayout.ObjectField(p.requiredVerticals[i], typeof(Vertical), false);
            if (GUILayout.Button("Delete")) {
                p.requiredVerticals.Remove(p.requiredVerticals[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Required Vertical")) {
            p.requiredVerticals.Add(null);
        }

        EditorGUILayout.LabelField("Required Infrastructure");
        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            p.requiredInfrastructure[t] = EditorGUILayout.IntField(t.ToString(), p.requiredInfrastructure[t]);
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
