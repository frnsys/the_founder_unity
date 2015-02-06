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
        c.disabled = EditorGUILayout.Toggle("Starts Disabled", c.disabled);
        c.description = EditorGUILayout.TextArea(c.description, GUILayout.Height(50));
        c.baseSizeLimit = EditorGUILayout.IntField("Worker Size Limit", c.baseSizeLimit);
        c.productLimit = EditorGUILayout.IntField("In-Market Product Limit", c.productLimit);
        c.cash.baseValue  = EditorGUILayout.FloatField("Starting Cash", c.cash.baseValue);

        EditorGUILayout.LabelField("Founder/CEO");
        if (c.founders.Count == 0) {
            Founder founder = (Founder)EditorGUILayout.ObjectField(null, typeof(Founder), false);
            c.founders.Add(founder);
        } else {
            c.founders[0] = (Founder)EditorGUILayout.ObjectField(c.founders[0], typeof(Founder), false);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bonuses and Unlocks");
        if (c.bonuses == null)
            c.bonuses = new EffectSet();
        EffectSetRenderer.RenderEffectSet(c, c.bonuses);
        EditorGUILayout.Space();

        EditorGUILayout.Space();

        // Locations
        EditorGUILayout.LabelField("Locations");
        for (int i=0; i < c.startLocations.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            c.startLocations[i] = (Location)EditorGUILayout.ObjectField(c.startLocations[i], typeof(Location), false);
            if (GUILayout.Button("Delete")) {
                c.startLocations.Remove(c.startLocations[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        Location newLocation = (Location)EditorGUILayout.ObjectField(null, typeof(Location), false);
        if (newLocation != null)
            c.startLocations.Add(newLocation);

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
