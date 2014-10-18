using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Consultancy))]
internal class ConsultancyInspector : Editor {

    private Consultancy c;

    public override void OnInspectorGUI() {
        c = target as Consultancy;

        c.name = EditorGUILayout.TextField("Name", c.name);
        c.description = EditorGUILayout.TextField("Bio", c.description);
        c.cost = EditorGUILayout.FloatField("Cost", c.cost);
        c.baseResearch = EditorGUILayout.FloatField("Base Research Points", c.baseResearch);
        c.researchTime = EditorGUILayout.IntField("Research Time", c.researchTime);

        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != c.name) {
                AssetDatabase.RenameAsset(path, c.name);
                AssetDatabase.Refresh();
            }
        }
    }

}
