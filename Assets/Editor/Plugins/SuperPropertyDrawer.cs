using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

/*
 * SuperPropertyDrawer
 * -------------------
 * This class makes it easier to deal with
 * Unity's PropertyDrawers and provides
 * a DrawList method for rendering lists of properties,
 * and a DrawReorderableList method for rendering lists of properties
 * using UnityEditorInternal.ReorderableList.
 *
 * DrawList is not as slick as the undocumented UnityEditorInternal.ReorderableList,
 * but does fine.
 */

abstract class SuperPropertyDrawer : PropertyDrawer {
    private float buttonWidth = 20;
    private float height = 0;
    private float padding = 6;

    // Have to keep track of lists so they are not created multiple times.
    private Dictionary<string, ReorderableList> lists = new Dictionary<string, ReorderableList>();

    // Implement in subclasses. This is called within OnGUI.
    public abstract Rect Edit(Rect position, SerializedProperty property);

    // Gets a cached ReorderableList. If one doesn't exist for the specified property,
    // one is created.
    public Rect DrawReorderableList(Rect position, SerializedProperty thisProp, SerializedProperty targetProp) {
        ReorderableList list;
        if (!lists.ContainsKey(targetProp.name)) {
            list = new UnityEditorInternal.ReorderableList(thisProp.serializedObject, targetProp, true, true, true, true);
            list.drawHeaderCallback += rect => GUI.Label(rect, targetProp.name);
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                EditorGUI.PropertyField(rect, 
                    list.serializedProperty.GetArrayElementAtIndex(index), 
                    GUIContent.none);
            };
            lists[targetProp.name] = list;
        } else {
            list = lists[targetProp.name];
        }
        list.DoList(position);
        position.y += list.GetHeight() + padding;
        return position;
    }


    // This is a jankier list than the UnityEditorInternal.ReorderableList,
    // but AFAIK it will better support property fields which are larger than one line.
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

        propertyRect = Edit(propertyRect, property);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        height = (propertyRect.y - position.y) + propertyRect.height;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return height;
    }
}
