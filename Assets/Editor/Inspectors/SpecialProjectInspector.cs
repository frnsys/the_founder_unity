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
        p.requiredProgress = EditorGUILayout.FloatField("Required Progress", p.requiredProgress);

        p.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", p.mesh, typeof(Mesh), false);
        p.texture = (Texture)EditorGUILayout.ObjectField("Texture", p.texture, typeof(Texture), false);

        EditorGUILayout.LabelField("Required Infrastructure");
        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            p.requiredInfrastructure[t] = EditorGUILayout.IntField(t.ToString(), p.requiredInfrastructure[t]);
        }

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
