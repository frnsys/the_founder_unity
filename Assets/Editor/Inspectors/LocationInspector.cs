using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(Location))]
internal class LocationInspector : Editor {

    Location i;

    void OnEnable() {
        i = target as Location;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;

        i.name = EditorGUILayout.TextField("Name", i.name);
        i.cost = EditorGUILayout.FloatField("Cost", i.cost);

        EditorGUILayout.LabelField("Description");
        i.description = EditorGUILayout.TextArea(i.description, GUILayout.Height(50));

        EditorGUILayout.LabelField("Infrastructure Capacity");

        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            i.capacity[t] =  EditorGUILayout.IntField(t.ToString(), i.capacity[t]);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"));
        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name != i.name) {
                AssetDatabase.RenameAsset(path, i.name);
                AssetDatabase.Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}
