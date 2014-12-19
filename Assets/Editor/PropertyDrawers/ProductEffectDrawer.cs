using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(ProductEffect))]
class ProductEffectDrawer : SuperPropertyDrawer {
    // The names of the list properties.
    private string[] listPropertyNames = new string[] {
        "productTypes",
        "verticals"
    };

    public override Rect Edit(Rect position, SerializedProperty property) {
        foreach (string key in listPropertyNames) {
            position = DrawReorderableList(position, property, property.FindPropertyRelative(key));
        }


        EditorGUI.PropertyField(position, property.FindPropertyRelative("buff"), GUIContent.none);
        position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("buff"), GUIContent.none, true);

        return position;
    }
}
