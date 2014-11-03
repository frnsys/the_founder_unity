using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Consultancy))]
internal class ConsultancyInspector : Editor {

    private Consultancy c;

    void OnEnable() {
        c = target as Consultancy;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;
        c.name = EditorGUILayout.TextField("Name", c.name);
        c.description = EditorGUILayout.TextField("Bio", c.description);
        c.cost = EditorGUILayout.FloatField("Cost", c.cost);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("research"));

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
