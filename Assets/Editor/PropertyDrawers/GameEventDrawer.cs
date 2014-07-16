using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(GameEvent))]
class GameEventDrawer : SuperPropertyDrawer {
    public override Rect Edit(Rect position, SerializedObject obj) {
        EditorGUI.PropertyField(position, obj.FindProperty("name"), GUIContent.none);
        position.y += EditorGUIUtility.singleLineHeight;
        //SerializedProperty outcomes = obj.FindProperty("outcomes");
        //position = DrawList(position, outcomes);
        return position;
    }
}
