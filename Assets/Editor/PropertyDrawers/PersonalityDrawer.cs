using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Personality))]
class PersonalityDrawer : SuperPropertyDrawer {

    public override Rect Edit(Rect position, SerializedProperty property) {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("aggression"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("cooperativeness"));
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("luck"));
        return position;
    }
}
