using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(StatBuff))]
class StatBuffDrawer : PropertyDrawer {
    string[] validStats = new string[] {
        "Happiness",
        "Productivity",
        "Probability",
        "Charisma",
        "Cleverness",
        "Creativity",
        "Design",
        "Marketing",
        "Engineering",
        "Cash"
    };

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
        propertyRect.width /= 4;

        int name_i = EditorGUI.Popup(propertyRect, Array.IndexOf(validStats, property.FindPropertyRelative("name").stringValue), validStats);
        if (name_i >= 0)
            property.FindPropertyRelative("name").stringValue = validStats[name_i];

        propertyRect.x += propertyRect.width;
        EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("value"), GUIContent.none);

        propertyRect.x += propertyRect.width;
        EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("type"), GUIContent.none);

        propertyRect.x += propertyRect.width;
        EditorGUI.PropertyField(propertyRect, property.FindPropertyRelative("duration"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
