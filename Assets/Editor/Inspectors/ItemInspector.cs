using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(Item))]
internal class ItemInspector : Editor {

    Item i;

    void OnEnable() {
        i = target as Item;
    }

    public override void OnInspectorGUI() {

        i.name = EditorGUILayout.TextField("Name", i.name);
        i.description = EditorGUILayout.TextField("Description", i.description);
        i.cost = EditorGUILayout.FloatField("Cost", i.cost);
        i.duration = EditorGUILayout.FloatField("Duration", i.duration);

        EditorGUILayout.LabelField("Store");
        i.store = (Store)EditorGUILayout.EnumPopup(i.store);
        EditorGUILayout.Space();

        i.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", i.mesh, typeof(Mesh), false);
        i.texture = (Texture)EditorGUILayout.ObjectField("Texture", i.texture, typeof(Texture), false);

        if (i.effects == null)
            i.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(i, i.effects);
        EditorGUILayout.Space();

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
