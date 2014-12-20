using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TechnologyManager : ManagerWindow<Technology> {
    [MenuItem("The Founder/Technology Manager")]
    static void Init() {
        TechnologyManager window = EditorWindow.CreateInstance<TechnologyManager>();
        window.targets = Technology.LoadAll();
        window.Show();
    }

    protected override string path {
        get { return "Assets/Resources/Technologies"; }
    }

    protected override void DrawInspector() {
        target.name = EditorGUILayout.TextField("Name", target.name);
        EditorGUILayout.Space();

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Description");
        target.description = EditorGUILayout.TextArea(target.description, GUILayout.Height(50));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Research Needed");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredResearch"), GUIContent.none);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Required Vertical");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredVertical"), GUIContent.none);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Required Technologies");
        for (int i=0; i < target.requiredTechnologies.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            target.requiredTechnologies[i] = (Technology)EditorGUILayout.ObjectField(target.requiredTechnologies[i], typeof(Technology));
            if (GUILayout.Button("Delete")) {
                target.requiredTechnologies.Remove(target.requiredTechnologies[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Required Technology")) {
            target.requiredTechnologies.Add(null);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Effects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUIContent.none);
        EditorGUILayout.Space();
    }
}
