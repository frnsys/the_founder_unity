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
        EditorGUILayout.LabelField("Description");
        w.description = EditorGUILayout.TextArea(w.description, GUILayout.Height(60));
        w.material = (Material)EditorGUILayout.ObjectField("Material", w.material, typeof(Material), false);
        w.productivity = EditorGUILayout.FloatField("Productivity", w.productivity);
        w.happiness = EditorGUILayout.FloatField("Happiness", w.happiness);
        w.charisma = EditorGUILayout.FloatField("Charisma", w.charisma);
        w.creativity = EditorGUILayout.FloatField("Creativity", w.creativity);
        w.cleverness = EditorGUILayout.FloatField("Cleverness", w.cleverness);
        w.robot = EditorGUILayout.Toggle("Is a Robot", w.robot);

        w.baseMinSalary = EditorGUILayout.FloatField("Base Minimum Salary", w.baseMinSalary);
        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            w.bio = Worker.BuildBio(w);
            w.personalInfo = Worker.BuildPreferences(w);

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
