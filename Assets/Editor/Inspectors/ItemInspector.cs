using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Item))]
internal class ItemInspector : Editor {

    string[] productStats = new string[] {
        "Appeal",
        "Usability",
        "Performance"
    };
    string[] workerStats = new string[] {
        "Happiness",
        "Productivity",
        "Charisma",
        "Creativity",
        "Cleverness",
        "Health",
        "Salary"
    };

    public override void OnInspectorGUI() {
        Item i = target as Item;

        i.name = EditorGUILayout.TextField("Name", i.name);
        i.cost = EditorGUILayout.FloatField("Cost", i.cost);
        i.duration = EditorGUILayout.FloatField("Duration", i.duration);
        EditorGUILayout.Space();

        // Product Types
        EditorGUILayout.LabelField("Product Types");
        EditorGUI.indentLevel = 1;
        for (int j=0; j < i.productTypes.Count; j++) {
            EditorGUILayout.BeginHorizontal();
            i.productTypes[j] = (ProductType)EditorGUILayout.EnumPopup(i.productTypes[j]);
            if (GUILayout.Button("Delete")) {
                i.productTypes.Remove(i.productTypes[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New Product Type")) {
            i.productTypes.Add(ProductType.Social_Network);
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();

        // Industries
        EditorGUILayout.LabelField("Industries");
        EditorGUI.indentLevel = 1;
        for (int j=0; j < i.industries.Count; j++) {
            EditorGUILayout.BeginHorizontal();
            i.industries[j] = (Industry)EditorGUILayout.EnumPopup(i.industries[j]);
            if (GUILayout.Button("Delete")) {
                i.industries.Remove(i.industries[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New Industry")) {
            i.industries.Add(Industry.Space);
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();

        // Markets
        EditorGUILayout.LabelField("Markets");
        EditorGUI.indentLevel = 1;
        for (int j=0; j < i.markets.Count; j++) {
            EditorGUILayout.BeginHorizontal();
            i.markets[j] = (Market)EditorGUILayout.EnumPopup(i.markets[j]);
            if (GUILayout.Button("Delete")) {
                i.markets.Remove(i.markets[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New Market")) {
            i.markets.Add(Market.Millenials);
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();

        // Product Buffs
        EditorGUILayout.LabelField("Product Buffs");
        EditorGUI.indentLevel = 1;
        for (int j=0; j < i.productBuffs.Count; j++) {
            EditorGUILayout.BeginHorizontal();

            int stat = EditorGUILayout.Popup(Array.IndexOf(productStats, i.productBuffs[j].name), productStats);

            i.productBuffs[j].name = productStats[stat];
            i.productBuffs[j].value = EditorGUILayout.FloatField(i.productBuffs[j].value);

            if (GUILayout.Button("Delete")) {
                i.productBuffs.Remove(i.productBuffs[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New Product Buff")) {
            i.productBuffs.Add(new StatBuff(productStats[0], 10));
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();

        // Worker Buffs
        EditorGUILayout.LabelField("Worker Buffs");
        EditorGUI.indentLevel = 1;
        for (int j=0; j < i.workerBuffs.Count; j++) {
            EditorGUILayout.BeginHorizontal();

            int stat = EditorGUILayout.Popup(Array.IndexOf(workerStats, i.workerBuffs[j].name), workerStats);

            i.workerBuffs[j].name = workerStats[stat];
            i.workerBuffs[j].value = EditorGUILayout.FloatField(i.workerBuffs[j].value);

            if (GUILayout.Button("Delete")) {
                i.workerBuffs.Remove(i.workerBuffs[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New Worker Buff")) {
            i.workerBuffs.Add(new StatBuff(workerStats[0], 10));
        }
        EditorGUI.indentLevel = 0;

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != i.name) {
                AssetDatabase.RenameAsset(path, i.name);
                AssetDatabase.Refresh();
            }
        }
    }

}
