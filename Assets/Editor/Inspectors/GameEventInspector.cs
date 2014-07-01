using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GameEvent))]
internal class GameEventInspector : Editor {

    bool effectsFoldout = true;
    string[] productStats = new string[] {
        "Appeal",
        "Usability",
        "Performance"
    };
    string[] productSubtypes = new string[] {
        "Product Type",
        "Industry",
        "Market"
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
        GameEvent ge = target as GameEvent;

        ge.name = EditorGUILayout.TextField("Name: ", ge.name);
        ge.probability.baseValue = EditorGUILayout.FloatField("Probability: ", ge.probability.baseValue);
        ge.probability.baseValue = Mathf.Clamp(ge.probability.baseValue, 0, 1);

        effectsFoldout = EditorGUILayout.Foldout(effectsFoldout, "Effects");
        if (effectsFoldout) {
            EditorGUI.indentLevel = 1;

            if (ge.effects == null) {
                ge.effects = new List<GameEffect>();
            }

            for (int i=0; i < ge.effects.Count; i++) {
                GameEffect effect = ge.effects[i];

                effect.type = (GameEffect.Type)EditorGUILayout.EnumPopup(effect.type);
                switch(effect.type) {
                    case GameEffect.Type.CASH:
                        SetupCashEffect(effect);
                        break;

                    case GameEffect.Type.ECONOMY:
                        // TO DO
                        effect.amount = EditorGUILayout.FloatField("Amount: ", effect.amount);
                        break;

                    case GameEffect.Type.PRODUCT:
                        SetupProductEffect(effect);
                        break;

                    case GameEffect.Type.WORKER:
                        SetupWorkerEffect(effect);
                        break;

                    case GameEffect.Type.EVENT:
                        // TO DO
                        break;

                    case GameEffect.Type.UNLOCK:
                        // TO DO
                        break;
                }

                EditorGUILayout.Space();
            }
            if (GUILayout.Button("Add New Effect")) {
                GameEffect effect = new GameEffect(GameEffect.Type.CASH, amount_ : 1000);
                //GameEffect effect = (GameEffect)ScriptableObject.CreateInstance<GameEffect>();
                effect.type = GameEffect.Type.CASH;
                effect.amount = 1000f;
                ge.effects.Add(effect);
                EditorUtility.SetDirty(target);
            }

            if (GUI.changed)
                EditorUtility.SetDirty(target);

                string path = AssetDatabase.GetAssetPath(target);
                if (name != ge.name) {
                    AssetDatabase.RenameAsset(path, ge.name);
                    AssetDatabase.Refresh();
                }

            EditorGUI.indentLevel = 0;
        }
    }

    private void SetupCashEffect(GameEffect effect) {
        // Reset unused.
        effect.id = 0;
        effect.subtype = null;
        effect.stat = null;

        effect.amount = EditorGUILayout.FloatField("Amount: ", effect.amount);
    }

    private void SetupProductEffect(GameEffect effect) {
        // Reset unused.
        effect.id = 0;

        // Default
        if (effect.subtype == null) {
            effect.subtype = productSubtypes[0];
        }
        int subtype = EditorGUILayout.Popup("Subtype: ", Array.IndexOf(productSubtypes, effect.subtype), productSubtypes);
        effect.subtype = productSubtypes[subtype];

        // Default
        if (effect.stat == null) {
            effect.stat = productStats[0];
        }
        int stat = EditorGUILayout.Popup("Stat: ", Array.IndexOf(productStats, effect.stat), productStats);
        effect.stat = productStats[stat];

        effect.amount = EditorGUILayout.FloatField("Amount: ", effect.amount);
    }

    private void SetupWorkerEffect(GameEffect effect) {
        // Reset unused.
        effect.id = 0;
        effect.subtype = null;

        // Default
        if (effect.stat == null) {
            effect.stat = workerStats[0];
        }
        int stat = EditorGUILayout.Popup("Stat: ", Array.IndexOf(workerStats, effect.stat), workerStats);
        effect.stat = workerStats[stat];

        effect.amount = EditorGUILayout.FloatField("Amount: ", effect.amount);
    }
}
