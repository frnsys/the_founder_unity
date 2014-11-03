using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Research))]
class ResearchDrawer : SuperPropertyDrawer {

    public override Rect Edit(Rect position, SerializedProperty property) {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("management"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("technical"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("design"));
        return position;
    }
}
