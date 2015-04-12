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

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
