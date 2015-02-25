using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(Perk))]
internal class PerkInspector : Editor {

    Perk i;

    void OnEnable() {
        i = target as Perk;
    }

    public override void OnInspectorGUI() {
        for (int k=0; k<i.upgrades.Count; k++) {
            Perk.Upgrade upgrade = i.upgrades[k];
            upgrade.name = EditorGUILayout.TextField("Name", upgrade.name);
            upgrade.description = EditorGUILayout.TextField("Description", upgrade.description);
            upgrade.cost = EditorGUILayout.FloatField("Cost", upgrade.cost);
            upgrade.requiredOffice = (Office.Type)EditorGUILayout.EnumPopup("Required Office", upgrade.requiredOffice);

            EditorGUILayout.LabelField("Required Technologies");
            for (int j=0; j < upgrade.requiredTechnologies.Count; j++) {
                EditorGUILayout.BeginHorizontal();
                upgrade.requiredTechnologies[j] = (Technology)EditorGUILayout.ObjectField(upgrade.requiredTechnologies[j], typeof(Technology), false);
                if (GUILayout.Button("Delete")) {
                    upgrade.requiredTechnologies.Remove(upgrade.requiredTechnologies[j]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Required Technology")) {
                upgrade.requiredTechnologies.Add(null);
            }

            upgrade.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", upgrade.mesh, typeof(Mesh), false);

            if (upgrade.effects == null)
                upgrade.effects = new EffectSet();
            EffectSetRenderer.RenderEffectSet(i, upgrade.effects);

            if (GUILayout.Button("Delete")) {
                i.upgrades.Remove(upgrade);
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("==========================");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }


        if (GUILayout.Button("Add Upgrade")) {
            i.upgrades.Add(new Perk.Upgrade());
            EditorUtility.SetDirty(target);
        }

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
