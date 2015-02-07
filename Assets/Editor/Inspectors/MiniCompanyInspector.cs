using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(MiniCompany))]
internal class MiniCompanyInspector : Editor {

    MiniCompany i;
    bool foldout = false;

    void OnEnable() {
        i = target as MiniCompany;
    }

    public override void OnInspectorGUI() {
        i.name = EditorGUILayout.TextField("Name", i.name);
        i.description = EditorGUILayout.TextField("Description", i.description);
        i.logo = (Texture)EditorGUILayout.ObjectField("Texture", i.logo, typeof(Texture), false);
        i.baseCost = EditorGUILayout.FloatField("Base Cost", i.baseCost);
        i.revenue = EditorGUILayout.FloatField("Revenue", i.revenue);
        i.aiCompany = (AICompany)EditorGUILayout.ObjectField("Associated AI Company", i.aiCompany, typeof(AICompany), false);

        if (i.effects == null)
            i.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(i, i.effects);
        foldout = EditorGUILayout.Foldout(foldout, "Unlocks");
        if (foldout)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").FindPropertyRelative("unlocks"), GUIContent.none);

        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name != i.name) {
                AssetDatabase.RenameAsset(path, i.name);
                AssetDatabase.Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}
