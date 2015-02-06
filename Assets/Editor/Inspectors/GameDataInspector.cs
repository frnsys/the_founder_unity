using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GameData))]
internal class GameDataInspector : Editor {

    private GameData gd;

    void OnEnable() {
        gd = target as GameData;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("Starting Unlocks");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlocked"), GUIContent.none);

        EditorGUILayout.LabelField("Other Companies");
        if (gd.otherCompanies == null)
            gd.otherCompanies = new List<AICompany>();

        for (int i=0; i < gd.otherCompanies.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            gd.otherCompanies[i] = (AICompany)EditorGUILayout.ObjectField(gd.otherCompanies[i], typeof(AICompany), false);
            if (GUILayout.Button("Delete")) {
                gd.otherCompanies.Remove(gd.otherCompanies[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        AICompany newOtherCompany = (AICompany)EditorGUILayout.ObjectField(null, typeof(AICompany), false);
        if (newOtherCompany != null)
            gd.otherCompanies.Add(newOtherCompany);

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
