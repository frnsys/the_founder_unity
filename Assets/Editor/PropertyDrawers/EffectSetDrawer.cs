using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(EffectSet))]
class EffectSetDrawer : SuperPropertyDrawer {

    // The names of the list properties.
    private string[] listPropertyNames = new string[] {
        "workers",
        "company"
    };

    public override Rect Edit(Rect position, SerializedProperty property) {
        foreach (string key in listPropertyNames) {
            position = DrawReorderableList(position, property, property.FindPropertyRelative(key));
        }

        EditorGUI.PropertyField(position, property.FindPropertyRelative("unlocks"));
        position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("unlocks"), GUIContent.none, true);

        position = EditorGUI.PrefixLabel(position, new GUIContent("products"));
        position = DrawList(position, property.FindPropertyRelative("products"));

        return position;
    }
}
