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

        EditorGUI.LabelField(position, "unlocks");
        position.y += 20;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("unlocks"), GUIContent.none);
        position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("unlocks"), GUIContent.none, true);

        EditorGUI.LabelField(position, "product buffs");
        position.y += 20;
        position = DrawList(position, property.FindPropertyRelative("products"));

        return position;
    }
}
