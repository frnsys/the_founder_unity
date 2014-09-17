using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Worker))]
internal class WorkerInspector : Editor {

    private Worker w;

    public override void OnInspectorGUI() {
        w = target as Worker;

        w.type = (WorkerType)EditorGUILayout.EnumPopup(w.type);
        w.name = EditorGUILayout.TextField("Name", w.name);
        w.bio = EditorGUILayout.TextField("Bio", w.bio);
        w.productivity.baseValue = EditorGUILayout.FloatField("Productivity", w.productivity.baseValue);
        w.happiness.baseValue = EditorGUILayout.FloatField("Happiness", w.happiness.baseValue);
        w.charisma.baseValue = EditorGUILayout.FloatField("Charisma", w.charisma.baseValue);
        w.creativity.baseValue = EditorGUILayout.FloatField("Creativity", w.creativity.baseValue);
        w.cleverness.baseValue = EditorGUILayout.FloatField("Cleverness", w.cleverness.baseValue);

        w.salary = EditorGUILayout.FloatField("Salary", w.salary);
        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != w.name) {
                AssetDatabase.RenameAsset(path, w.name);
                AssetDatabase.Refresh();
            }
        }
    }

}
