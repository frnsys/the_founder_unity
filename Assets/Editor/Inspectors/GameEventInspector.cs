using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    List<ProductType> productTypes;
    string[] pt_o;

    List<Industry> industries;
    string[] i_o;

    List<Market> markets;
    string[] m_o;

    void OnEnable() {
        ge = target as GameEvent;

        productTypes = ProductType.LoadAll();
        pt_o = productTypes.Select(j => j.ToString()).ToArray();

        industries = Industry.LoadAll();
        i_o = industries.Select(j => j.ToString()).ToArray();

        markets = Market.LoadAll();
        m_o = markets.Select(j => j.ToString()).ToArray();
    }

    public override void OnInspectorGUI() {
        ge.name = EditorGUILayout.TextField("Name", ge.name);

        EditorGUILayout.LabelField("Description");
        ge.description = EditorGUILayout.TextArea(ge.description, GUILayout.Height(50));

        ge.probability.baseValue = EditorGUILayout.FloatField("Probability", ge.probability.baseValue);
        ge.probability.baseValue = Mathf.Clamp(ge.probability.baseValue, 0, 1);

        foldouts["effects"] = EditorGUILayout.Foldout(foldouts["effects"], "Effects");
        if (foldouts["effects"]) {
            EditorGUI.indentLevel = 1;

            for (int i=0; i < ge.effects.Count; i++) {
                GameEffect e = ge.effects[i];

                // Deletion
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("An Effect");
                if (GUILayout.Button("Delete")) {
                    ge.effects.Remove(e);
                }
                EditorGUILayout.EndHorizontal();

                // Product Types
                EditorGUILayout.LabelField("Product Effects");
                EditorGUI.indentLevel += 1;

                EditorGUILayout.LabelField("Product Types");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < e.productTypes.Count; j++) {
                    EditorGUILayout.BeginHorizontal();

                    int pt_i = EditorGUILayout.Popup(Array.IndexOf(pt_o, e.productTypes[j].ToString()), pt_o);
                    e.productTypes[j] = productTypes[pt_i];

                    if (GUILayout.Button("Delete")) {
                        e.productTypes.Remove(e.productTypes[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add New Product Type", GUILayout.ExpandWidth(false))) {
                    e.productTypes.Add(productTypes[0]);
                }
                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();

                // Industries
                EditorGUILayout.LabelField("Industries");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < e.industries.Count; j++) {
                    EditorGUILayout.BeginHorizontal();

                    int i_i = EditorGUILayout.Popup(Array.IndexOf(i_o, e.industries[j].ToString()), i_o);
                    e.industries[j] = industries[i_i];

                    if (GUILayout.Button("Delete")) {
                        e.industries.Remove(e.industries[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add New Industry", GUILayout.ExpandWidth(false))) {
                    e.industries.Add(industries[0]);
                }
                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();

                // Markets
                EditorGUILayout.LabelField("Markets");
                EditorGUI.indentLevel += 1;
                for (int j=0; j < e.markets.Count; j++) {
                    EditorGUILayout.BeginHorizontal();

                    int m_i = EditorGUILayout.Popup(Array.IndexOf(m_o, e.markets[j].ToString()), m_o);
                    e.markets[j] = markets[m_i];

                    if (GUILayout.Button("Delete")) {
                        e.markets.Remove(e.markets[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add New Market", GUILayout.ExpandWidth(false))) {
                    e.markets.Add(markets[0]);
                }
                EditorGUI.indentLevel -= 1;

                // Product Buffs
                BuffFields("Product", e.productBuffs);
                EditorGUILayout.Space();
                EditorGUI.indentLevel -= 1;


                // Worker Buffs
                BuffFields("Worker", e.workerBuffs);

                // Company Buffs
                BuffFields("Company", e.companyBuffs);
            }


            // New Effect
            if (GUILayout.Button("Add New Effect")) {
                ge.effects.Add(new GameEffect());
                EditorUtility.SetDirty(target);
            }

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

    private void BuffFields(string name, List<StatBuff> buffs) {
        string key = name.ToLower();

        EditorGUILayout.LabelField(name + " Buffs");
        EditorGUI.indentLevel += 1;

        for (int j=0; j < buffs.Count; j++) {
            EditorGUILayout.BeginHorizontal();
            int stat = EditorGUILayout.Popup(Array.IndexOf(stats[key], buffs[j].name), stats[key]);
            buffs[j].name = stats[key][stat];

            buffs[j].value = EditorGUILayout.FloatField(buffs[j].value);
            if (GUILayout.Button("Delete")) {
                buffs.Remove(buffs[j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add New " + name + " Buff", GUILayout.ExpandWidth(false))) {
            buffs.Add(new StatBuff(stats[key][0], 0));
        }
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.Space();
    }
}
