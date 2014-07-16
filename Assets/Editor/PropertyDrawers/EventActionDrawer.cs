using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(EventAction))]
class EventActionDrawer : SuperPropertyDrawer {
    public override Rect Edit(Rect position, SerializedProperty property) {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("name"), GUIContent.none);
        position.y += EditorGUIUtility.singleLineHeight;
        position = DrawReorderableList(position, property, property.FindPropertyRelative("outcomes"));

        return position;
    }
}
