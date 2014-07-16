using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/*
 * SuperPropertyDrawer
 * -------------------
 * This class makes it easier to deal with
 * Unity's PropertyDrawers and provides
 * a DrawList method for rendering lists of properties.
 *
 * It's not as slick as the undocumented UnityEditorInternal.ReorderableList,
 * but does fine and is easier to use.
 */

abstract class SuperPropertyDrawer : PropertyDrawer {
    private float buttonWidth = 20;
    private float height = 0;

    // Implement in subclasses. This is called within OnGUI.
    public abstract Rect Edit(Rect position, SerializedObject obj);

    public Rect DrawList(Rect position, SerializedProperty prop) {
        Rect propertyRect = position;
        for (int i=0; i<prop.arraySize; i++) {
            propertyRect.width -= buttonWidth;
            propertyRect.height = EditorGUI.GetPropertyHeight(prop.GetArrayElementAtIndex(i), GUIContent.none, true);

            EditorGUI.PropertyField(propertyRect, prop.GetArrayElementAtIndex(i), GUIContent.none, true);

            propertyRect.x += propertyRect.width;
            propertyRect.width = buttonWidth;
            if (GUI.Button(propertyRect, "-")) {
                int oldSize = prop.arraySize;
                prop.DeleteArrayElementAtIndex(i);
                if (prop.arraySize == oldSize) {
                    prop.DeleteArrayElementAtIndex(i);
                }
            }

            propertyRect.y += propertyRect.height;

            // Reset positioning for the next line.
            propertyRect.x = position.x;
            propertyRect.width = position.width;
        }

        propertyRect.height = EditorGUIUtility.singleLineHeight;
        propertyRect.y += EditorGUIUtility.singleLineHeight;

        propertyRect.x = propertyRect.x + propertyRect.width - buttonWidth;
        propertyRect.width = buttonWidth;
        if (GUI.Button(propertyRect, "+")) {
            prop.arraySize += 1;
        }
        propertyRect.width = position.width;
        propertyRect.x = position.x;

        // Apply this list's height to the passed position.
        position.y += (propertyRect.y - position.y) + propertyRect.height;

        return position;
    }

    // Draw the property inside the given rect
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
        serializedObject.Update();

        // Reset the height.
        height = 0;

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

        propertyRect = Edit(propertyRect, serializedObject);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        serializedObject.ApplyModifiedProperties();

        height = (propertyRect.y - position.y) + propertyRect.height;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return height;
    }
}
