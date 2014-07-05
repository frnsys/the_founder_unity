using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Market))]
internal class MarketInspector : Editor {

    public override void OnInspectorGUI() {
        Market p = target as Market;

        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.LabelField("Market");
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
