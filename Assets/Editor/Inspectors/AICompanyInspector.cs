using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(AICompany))]
internal class AICompanyInspector : Editor {

    private AICompany c;

    void OnEnable() {
        c = target as AICompany;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;
        c.name = EditorGUILayout.TextField("Name", c.name);
        EditorGUILayout.LabelField("Description");
        c.description = EditorGUILayout.TextArea(c.description, GUILayout.Height(50));
        c.baseSizeLimit = EditorGUILayout.IntField("Worker Size Limit", c.baseSizeLimit);
        c.cash.baseValue  = EditorGUILayout.FloatField("Starting Cash", c.cash.baseValue);

        EditorGUILayout.LabelField("Founder/CEO");
        if (c.founders.Count == 0) {
            Founder founder = (Founder)EditorGUILayout.ObjectField(null, typeof(Founder));
            c.founders.Add(founder);
        } else {
            c.founders[0] = (Founder)EditorGUILayout.ObjectField(c.founders[0], typeof(Founder));
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Personality");
        c.aggression = EditorGUILayout.FloatField("Aggression", c.aggression);
        c.cooperativeness = EditorGUILayout.FloatField("Cooperativeness", c.cooperativeness);
        c.luck = EditorGUILayout.FloatField("Luck", c.luck);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bonuses and Unlocks");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bonuses"), GUIContent.none);

        EditorGUILayout.Space();

        // Workers
        EditorGUILayout.LabelField("Starting Workers");
        for (int i=0; i < c.startWorkers.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            c.startWorkers[i] = (Worker)EditorGUILayout.ObjectField(c.startWorkers[i], typeof(Worker));
            if (GUILayout.Button("Delete")) {
                c.startWorkers.Remove(c.startWorkers[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (c.startWorkers.Count < c.sizeLimit) {
            Worker newWorker = (Worker)EditorGUILayout.ObjectField(null, typeof(Worker));
            if (newWorker != null)
                c.startWorkers.Add(newWorker);
        }

        EditorGUILayout.Space();

        // Products
        EditorGUILayout.LabelField("Starting Products");
        for (int i=0; i < c.startProducts.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            c.startProducts[i] = (Product)EditorGUILayout.ObjectField(c.startProducts[i], typeof(Product));
            if (GUILayout.Button("Delete")) {
                c.startProducts.Remove(c.startProducts[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (c.startProducts.Count < c.sizeLimit) {
            Product newProduct = (Product)EditorGUILayout.ObjectField(null, typeof(Product));
            if (newProduct != null)
                c.startProducts.Add(newProduct);
        }

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
