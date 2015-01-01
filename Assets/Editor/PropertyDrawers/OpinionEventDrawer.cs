using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(OpinionEvent))]
class OpinionEventDrawer : SuperPropertyDrawer {

    // Draw the property inside the given rect
    public override Rect Edit(Rect position, SerializedProperty property) {

        EditorGUI.LabelField(position, "Opinion Event");
        position.y += 20;
        EditorGUI.LabelField(position, "Name");
        position.y += 20;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), GUIContent.none);
        position.y += 20;
        EditorGUI.LabelField(position, "Opinion Effect");
        position.y += 20;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("opinion").FindPropertyRelative("value"), GUIContent.none);
        position.y += 20;
        EditorGUI.LabelField(position, "Publicity Effect");
        position.y += 20;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("publicity").FindPropertyRelative("value"), GUIContent.none);
        position.y += 20;
        return position;
    }
}
