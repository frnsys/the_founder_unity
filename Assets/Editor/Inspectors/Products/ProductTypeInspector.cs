using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(ProductType))]
internal class ProductTypeInspector : Editor {

    public override void OnInspectorGUI() {
        ProductType p = target as ProductType;

        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.LabelField("Product Type");
        p.name = EditorGUILayout.TextField("Name", p.name);
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
