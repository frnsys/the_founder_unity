using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BulkWorkerTool : EditorWindow {
    [MenuItem("The Founder/Bulk Worker Tool")]
    static void Init() {
        BulkWorkerTool window = EditorWindow.CreateInstance<BulkWorkerTool>();
        window.Show();
    }

    void OnGUI() {
        string workers_raw = "";
        EditorStyles.textField.wordWrap = true;
        workers_raw = EditorGUILayout.TextArea(workers_raw, GUILayout.Height(300));

        if (workers_raw != "") {
            List<Material> materials = Resources.LoadAll<Material>("Models/Employees/Materials/Human").ToList();

            string[] workers_strings = workers_raw.Split('\n');
            foreach (string w_s in workers_strings) {
                string[] worker_attrs = w_s.Split('|');
                string name = worker_attrs[0];
                string title = worker_attrs[1];
                float baseMinSalary = float.Parse(worker_attrs[2]);
                float happiness = float.Parse(worker_attrs[3]);
                float productivity = float.Parse(worker_attrs[4]);
                float charisma = float.Parse(worker_attrs[5]);
                float creativity = float.Parse(worker_attrs[6]);
                float cleverness = float.Parse(worker_attrs[7]);

                Worker worker = ScriptableObject.CreateInstance<Worker>();
                worker.name = name;
                worker.title = title;
                worker.baseMinSalary = baseMinSalary;
                worker.happiness = happiness;
                worker.productivity = productivity;
                worker.charisma = charisma;
                worker.creativity = creativity;
                worker.cleverness = cleverness;
                worker.bio = Worker.BuildBio(worker);
                worker.personalInfo = Worker.BuildPreferences(worker);

                worker.material = materials[UnityEngine.Random.Range(0, materials.Count - 1)];
                AssetDatabase.CreateAsset(worker, "Assets/Resources/Workers/Bulk/" + name + ".asset");
            }
            AssetDatabase.SaveAssets();
        }
        workers_raw = "";
    }
}
