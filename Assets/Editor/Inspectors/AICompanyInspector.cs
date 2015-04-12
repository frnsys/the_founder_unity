using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(AICompany))]
internal class AICompanyInspector : Editor {

    private AICompany c;

    void OnEnable() {
        c = target as AICompany;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;
        c.name = EditorGUILayout.TextField("Name", c.name);
        EditorGUILayout.LabelField("Description");
        c.description = EditorGUILayout.TextArea(c.description, GUILayout.Height(50));
        c.slogan = EditorGUILayout.TextField("Slogan", c.slogan);
        c.disabled = EditorGUILayout.Toggle("Starts Disabled", c.disabled);

        c.designSkill = EditorGUILayout.IntField("Design Skill", c.designSkill);
        c.engineeringSkill = EditorGUILayout.IntField("Engineering Skill", c.engineeringSkill);
        c.marketingSkill = EditorGUILayout.IntField("Marketing Skill", c.marketingSkill);

        EditorGUILayout.LabelField("Founder/CEO");
        c.founder = (Worker)EditorGUILayout.ObjectField(c.founder, typeof(Worker), false);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bonuses and Unlocks");
        if (c.bonuses == null)
            c.bonuses = new EffectSet();
        EffectSetRenderer.RenderEffectSet(c, c.bonuses);
        EditorGUILayout.Space();

        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != c.name) {
                AssetDatabase.RenameAsset(path, c.name);
                AssetDatabase.Refresh();
            }
        }
    }

}
