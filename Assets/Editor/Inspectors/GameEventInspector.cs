using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GameEvent))]
internal class GameEventInspector : Editor {

    GameEvent ge;

    void OnEnable() {
        ge = target as GameEvent;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUILayoutOption[] options = { GUILayout.MaxWidth(320.0f), GUILayout.MinWidth(10.0f), GUILayout.ExpandHeight(true) };
        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Description");
        ge.description = EditorGUILayout.TextArea(ge.description, options);
        ge.repeatable = EditorGUILayout.Toggle("Repeatable", ge.repeatable);
        ge.from = EditorGUILayout.TextField("From (Email or Publication)", ge.from, options);
        ge.type = (GameEvent.Type)EditorGUILayout.EnumPopup("Type", ge.type, options);
        ge.image = (Texture)EditorGUILayout.ObjectField("Image", ge.image, typeof(Texture), false, options);
        ge.probability = EditorGUILayout.FloatField("Probability", ge.probability, options);
        ge.probability = Mathf.Clamp(ge.probability, 0, 1);

        if (ge.effects == null)
            ge.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(ge, ge.effects);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").FindPropertyRelative("unlocks"), GUIContent.none);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("conditions"), true, options);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("actions"), true, options);

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawCustomList<T>(List<T> list, string propertyName) {
        for (int i=0; i < list.Count; i++) {
            var e = list[i];

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete")) {
                list.Remove(e);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName).GetArrayElementAtIndex(i), GUIContent.none, true);
        }
    }

    private ReorderableList ShowList(SerializedProperty property, string labelText) {
        ReorderableList list = new UnityEditorInternal.ReorderableList(serializedObject, property, true, true, true, true);
        list.drawHeaderCallback += rect => GUI.Label(rect, labelText);
        list.drawElementCallback += (rect, index, active, focused) =>
        {
            rect.height = 16;
            rect.y += 2;
            EditorGUI.PropertyField(rect,
                list.serializedProperty.GetArrayElementAtIndex(index),
                GUIContent.none);
        };
        return list;
    }

}
