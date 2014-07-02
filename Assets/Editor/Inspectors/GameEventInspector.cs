using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GameEvent))]
internal class GameEventInspector : Editor {

    Dictionary<string, bool> foldouts = new Dictionary<string, bool> {
        {"effects", true},
        {"product", true},
        {"worker", true},
        {"company", true},
        {"economy", true}
    };

    Dictionary<string, string[]> stats = new Dictionary<string, string[]> {
        {"product", new string[] {
            "Appeal",
            "Usability",
            "Performance"
        }},
        {"worker", new string[] {
            "Happiness",
            "Productivity",
            "Charisma",
            "Creativity",
            "Cleverness",
            "Health",
            "Salary"
        }},
        {"company", new string[] {
            "Cash"
        }},
        {"economy", new string[] {
            "Unemployment"
        }}
    };


    GameEvent ge;
    public override void OnInspectorGUI() {
        ge = target as GameEvent;

        ge.name = EditorGUILayout.TextField("Name", ge.name);
        ge.probability.baseValue = EditorGUILayout.FloatField("Probability", ge.probability.baseValue);
        ge.probability.baseValue = Mathf.Clamp(ge.probability.baseValue, 0, 1);

        foldouts["effects"] = EditorGUILayout.Foldout(foldouts["effects"], "Effects");
        if (foldouts["effects"]) {
            EditorGUI.indentLevel = 1;

            ProductEffects();
            WorkerEffects();
            CompanyEffects();
            EconomyEffects();

            if (GUI.changed) {
                EditorUtility.SetDirty(target);

                string path = AssetDatabase.GetAssetPath(target);
                string name = Path.GetFileNameWithoutExtension(path);
                if (name != ge.name) {
                    AssetDatabase.RenameAsset(path, ge.name);
                    AssetDatabase.Refresh();
                }
            }

            EditorGUI.indentLevel = 0;
        }
    }

    private void EconomyEffects() {
        foldouts["economy"] = EditorGUILayout.Foldout(foldouts["economy"], "Economy Effects");
        if (foldouts["economy"]) {
            EditorGUI.indentLevel += 1;

            for (int i=0; i < ge.economyEffects.Count; i++) {
                EconomyEffect w = ge.economyEffects[i];

                // DELETION --------------------------
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("-------------------------");
                if (GUILayout.Button("Delete")) {
                    ge.economyEffects.Remove(w);
                }
                EditorGUILayout.EndHorizontal();

                // BUFFS --------------------------
                for (int j=0; j < w.buffs.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    int stat = EditorGUILayout.Popup(Array.IndexOf(stats["economy"], w.buffs[j].name), stats["economy"]);
                    w.buffs[j].name = stats["economy"][stat];

                    w.buffs[j].value = EditorGUILayout.FloatField(w.buffs[j].value);

                    if (GUILayout.Button("Delete")) {
                        w.buffs.Remove(w.buffs[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Buff", GUILayout.ExpandWidth(false))) {
                    w.buffs.Add(new StatBuff(stats["economy"][0], 0));
                }
                EndCenter();

                EditorGUILayout.Space();
            }

            // ADD NEW --------------------------
            if (GUILayout.Button("Add New Economy Effect")) {
                ge.economyEffects.Add(new EconomyEffect());
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }



    private void CompanyEffects() {
        foldouts["company"] = EditorGUILayout.Foldout(foldouts["company"], "Company Effects");
        if (foldouts["company"]) {
            EditorGUI.indentLevel += 1;

            for (int i=0; i < ge.companyEffects.Count; i++) {
                CompanyEffect w = ge.companyEffects[i];

                // DELETION --------------------------
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("-------------------------");
                if (GUILayout.Button("Delete")) {
                    ge.companyEffects.Remove(w);
                }
                EditorGUILayout.EndHorizontal();

                // BUFFS --------------------------
                for (int j=0; j < w.buffs.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    int stat = EditorGUILayout.Popup(Array.IndexOf(stats["company"], w.buffs[j].name), stats["company"]);
                    w.buffs[j].name = stats["company"][stat];

                    w.buffs[j].value = EditorGUILayout.FloatField(w.buffs[j].value);

                    if (GUILayout.Button("Delete")) {
                        w.buffs.Remove(w.buffs[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Buff", GUILayout.ExpandWidth(false))) {
                    w.buffs.Add(new StatBuff(stats["company"][0], 0));
                }
                EndCenter();

                EditorGUILayout.Space();
            }

            // ADD NEW --------------------------
            if (GUILayout.Button("Add New Company Effect")) {
                ge.companyEffects.Add(new CompanyEffect());
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }



    private void WorkerEffects() {
        foldouts["worker"] = EditorGUILayout.Foldout(foldouts["worker"], "Worker Effects");
        if (foldouts["worker"]) {
            EditorGUI.indentLevel += 1;

            for (int i=0; i < ge.workerEffects.Count; i++) {
                WorkerEffect w = ge.workerEffects[i];

                // DELETION --------------------------
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("-------------------------");
                if (GUILayout.Button("Delete")) {
                    ge.workerEffects.Remove(w);
                }
                EditorGUILayout.EndHorizontal();

                // BUFFS --------------------------
                for (int j=0; j < w.buffs.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    int stat = EditorGUILayout.Popup(Array.IndexOf(stats["worker"], w.buffs[j].name), stats["worker"]);
                    w.buffs[j].name = stats["worker"][stat];

                    w.buffs[j].value = EditorGUILayout.FloatField(w.buffs[j].value);

                    if (GUILayout.Button("Delete")) {
                        w.buffs.Remove(w.buffs[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Buff", GUILayout.ExpandWidth(false))) {
                    w.buffs.Add(new StatBuff(stats["worker"][0], 0));
                }
                EndCenter();

                EditorGUILayout.Space();
            }

            // ADD NEW --------------------------
            if (GUILayout.Button("Add New Worker Effect")) {
                ge.workerEffects.Add(new WorkerEffect());
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }

    private void ProductEffects() {
        foldouts["product"] = EditorGUILayout.Foldout(foldouts["product"], "Product Effects");
        if (foldouts["product"]) {
            EditorGUI.indentLevel += 1;

            for (int i=0; i < ge.productEffects.Count; i++) {
                ProductEffect p = ge.productEffects[i];

                // DELETION --------------------------
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("-------------------------");
                if (GUILayout.Button("Delete")) {
                    ge.productEffects.Remove(p);
                }
                EditorGUILayout.EndHorizontal();

                // PRODUCT TYPES --------------------------
                EditorGUILayout.LabelField("Product Types");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < p.productTypes.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    p.productTypes[j] = (ProductType)EditorGUILayout.EnumPopup(p.productTypes[j]);
                    if (GUILayout.Button("Delete")) {
                        p.productTypes.Remove(p.productTypes[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Product Type", GUILayout.ExpandWidth(false))) {
                    p.productTypes.Add(ProductType.Social_Network);
                }
                EndCenter();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();

                // INDUSTRIES --------------------------
                EditorGUILayout.LabelField("Industries");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < p.industries.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    p.industries[j] = (Industry)EditorGUILayout.EnumPopup(p.industries[j]);
                    if (GUILayout.Button("Delete")) {
                        p.industries.Remove(p.industries[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Industry", GUILayout.ExpandWidth(false))) {
                    p.industries.Add(Industry.Space);
                }
                EndCenter();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();

                // MARKETS --------------------------
                EditorGUILayout.LabelField("Markets");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < p.markets.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    p.markets[j] = (Market)EditorGUILayout.EnumPopup(p.markets[j]);
                    if (GUILayout.Button("Delete")) {
                        p.markets.Remove(p.markets[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Market", GUILayout.ExpandWidth(false))) {
                    p.markets.Add(Market.Millenials);
                }
                EndCenter();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();

                // BUFFS --------------------------
                EditorGUILayout.LabelField("Buffs");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < p.buffs.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    int stat = EditorGUILayout.Popup(Array.IndexOf(stats["product"], p.buffs[j].name), stats["product"]);
                    p.buffs[j].name = stats["product"][stat];

                    p.buffs[j].value = EditorGUILayout.FloatField(p.buffs[j].value);

                    if (GUILayout.Button("Delete")) {
                        p.buffs.Remove(p.buffs[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                BeginCenter();
                if (GUILayout.Button("Add New Buff", GUILayout.ExpandWidth(false))) {
                    p.buffs.Add(new StatBuff(stats["product"][0], 0));
                }
                EndCenter();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();
            }

            // ADD NEW --------------------------
            if (GUILayout.Button("Add New Product Effect")) {
                ge.productEffects.Add(new ProductEffect());
            }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }

    private void BeginCenter() {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }
    private void EndCenter() {
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
}
