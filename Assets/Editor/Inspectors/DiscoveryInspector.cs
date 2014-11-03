using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Discovery))]
internal class DiscoveryInspector : Editor {

    Discovery d;

    void OnEnable() {
        d = target as Discovery;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Description");
        d.description = EditorGUILayout.TextArea(d.description, GUILayout.Height(50));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Research Needed");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredResearch"), GUIContent.none);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Effects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUIContent.none);
        EditorGUILayout.Space();


        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
