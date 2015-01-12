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

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Description");
        ge.description = EditorGUILayout.TextArea(ge.description, GUILayout.Height(50));
        EditorGUILayout.Space();

        ge.probability = EditorGUILayout.FloatField("Probability", ge.probability);
        ge.probability = Mathf.Clamp(ge.probability, 0, 1);
        EditorGUILayout.Space();

        if (ge.effects.effects == null)
            ge.effects.effects = new List<IEffect>();
        EffectSetRenderer.RenderEffectSet(ge, ge.effects);
        EditorGUILayout.Space();


        // Actions
        // Have to handle this one specially cause nested lists are tricky...if not impossible.
        EditorGUILayout.LabelField("Actions");
        EditorGUI.indentLevel += 1;
        if (ge.actions == null) {
            ge.actions = new List<EventAction>();
        }
        DrawCustomList<EventAction>(ge.actions, "actions");
        if (GUILayout.Button("Add New Action")) {
            ge.actions.Add(new EventAction("Some Action Name", new List<GameEvent>(), 0));
            EditorUtility.SetDirty(target);
        }
        EditorGUI.indentLevel -= 1;

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
