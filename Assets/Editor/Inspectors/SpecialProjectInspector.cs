using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(SpecialProject))]
internal class SpecialProjectInspector : Editor {

    SpecialProject p;

    void OnEnable() {
        p = target as SpecialProject;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        p.name = EditorGUILayout.TextField("Name", p.name);
        p.description = EditorGUILayout.TextField("Description", p.description);
        p.cost = EditorGUILayout.FloatField("Cost", p.cost);
        p.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", p.mesh, typeof(Mesh), false);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredProducts"), true);

        EditorGUILayout.LabelField("Effects");
        if (p.effects == null)
            p.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(p, p.effects);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").FindPropertyRelative("unlocks"), GUIContent.none);
        EditorGUILayout.Space();

        // Let Unity know to save on changes.
        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != p.ToString()) {
                AssetDatabase.RenameAsset(path, p.ToString());
                AssetDatabase.Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}
