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

        EditorStyles.textField.wordWrap = true;
        w.name = EditorGUILayout.TextField("Name", w.name);
        w.title = EditorGUILayout.TextField("Title", w.title);
        EditorGUILayout.LabelField("Bio (auto-generated, changes not saved)");
        EditorGUILayout.TextArea(w.bio, GUILayout.Height(60));
        EditorGUILayout.LabelField("Description");
        w.description = EditorGUILayout.TextArea(w.description, GUILayout.Height(60));
        w.productivity.baseValue = EditorGUILayout.FloatField("Productivity", w.productivity.baseValue);
        w.happiness.baseValue = EditorGUILayout.FloatField("Happiness", w.happiness.baseValue);
        w.charisma.baseValue = EditorGUILayout.FloatField("Charisma", w.charisma.baseValue);
        w.creativity.baseValue = EditorGUILayout.FloatField("Creativity", w.creativity.baseValue);
        w.cleverness.baseValue = EditorGUILayout.FloatField("Cleverness", w.cleverness.baseValue);
        w.robot = EditorGUILayout.Toggle("Is a Robot", w.robot);

        w.salary = EditorGUILayout.FloatField("Salary", w.salary);
        w.baseMinSalary = EditorGUILayout.FloatField("Base Minimum Salary", w.baseMinSalary);
        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            w.bio = Worker.BuildBio(w);

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
