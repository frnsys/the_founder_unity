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

        p.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", p.mesh, typeof(Mesh), false);

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
