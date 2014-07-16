using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(UnlockSet))]
class UnlockSetDrawer : PropertyDrawer {

    // This is so heinous, but Unity is fucking awful
    // when it comes to GUI stuff.
    private Dictionary<string, ReorderableList> lists = new Dictionary<string, ReorderableList>();

    // The names of the list properties.
    private string[] listPropertyNames = new string[] {
        "industries",
        "markets",
        "productTypes",
        "workers",
        "events"
    };

    private float padding = 6;

    private ReorderableList GetList(SerializedProperty property, string propertyName) {
        if (!lists.ContainsKey(propertyName)) {
            ReorderableList list = new UnityEditorInternal.ReorderableList(property.serializedObject, property.FindPropertyRelative(propertyName), true, true, true, true);
            list.drawHeaderCallback += rect => GUI.Label(rect, propertyName);
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                EditorGUI.PropertyField(rect, 
                    list.serializedProperty.GetArrayElementAtIndex(index), 
                    GUIContent.none);
            };
            lists[propertyName] = list;
        }
        return lists[propertyName];
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

        foreach (string key in listPropertyNames)
        {
            ReorderableList list = GetList(property, key);
            list.DoList(position);
            position.y += list.GetHeight() + padding;
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = 0;
        foreach (string key in listPropertyNames)
        {
            ReorderableList list = GetList(property, key);
            height += list.GetHeight() + padding;
        }
        return height;
    }
}
