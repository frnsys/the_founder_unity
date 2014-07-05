using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Industry))]
internal class IndustryInspector : Editor {

    public override void OnInspectorGUI() {
        Industry p = target as Industry;

        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.LabelField("Industry");
        p.name = EditorGUILayout.TextField(p.name);
        p.description = EditorGUILayout.TextArea(p.description, GUILayout.Height(50));

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
        }
    }

}
