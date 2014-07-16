using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(EventAction))]
class EventActionDrawer : PropertyDrawer {
    private ReorderableList outcomesList;
    private float padding = 6;

    private ReorderableList GetList(SerializedProperty property) {
        if (outcomesList == null) {
            outcomesList = new UnityEditorInternal.ReorderableList(property.serializedObject, property.FindPropertyRelative("outcomes"), true, true, true, true);
            outcomesList.drawHeaderCallback += rect => GUI.Label(rect, "Outcomes");
            outcomesList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                EditorGUI.PropertyField(rect, 
                    outcomesList.serializedProperty.GetArrayElementAtIndex(index), 
                    GUIContent.none);
            };
        }
        return outcomesList;
    }

    // Draw the property inside the given rect
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        position = EditorGUI.PrefixLabel(position, label);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect propertyRect = position;
        propertyRect.height = EditorGUIUtility.singleLineHeight;

        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue as EventAction);
        EditorGUI.PropertyField(propertyRect, serializedObject.FindProperty("name"), GUIContent.none);

        SerializedProperty outcomes = serializedObject.FindProperty("outcomes");
        for (int i=0; i<outcomes.arraySize; i++) {
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(propertyRect, outcomes.GetArrayElementAtIndex(i), GUIContent.none);
        }

        propertyRect.y += EditorGUIUtility.singleLineHeight;
        if (GUI.Button(propertyRect, "Add New Outcome")) {
            int i = outcomes.arraySize;
            outcomes.InsertArrayElementAtIndex(i);
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 40;
        //return GetList(property).GetHeight() + padding;
    }
}
