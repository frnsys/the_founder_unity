using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(Location))]
internal class LocationInspector : Editor {

    Location i;

    void OnEnable() {
        i = target as Location;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorStyles.textField.wordWrap = true;

        i.name = EditorGUILayout.TextField("Name", i.name);
        i.cost = EditorGUILayout.FloatField("Cost", i.cost);

        EditorGUILayout.LabelField("Description");
        i.description = EditorGUILayout.TextArea(i.description, GUILayout.Height(50));

        EditorGUILayout.LabelField("Market Region");
        i.market = (MarketManager.Market)EditorGUILayout.EnumPopup(i.market);
        EditorGUILayout.Space();

        i.rotation = (Vector3)EditorGUILayout.Vector3Field("Coordinates RotXYZ", i.rotation);
        EditorGUILayout.Space();

        i.altMesh = (Mesh)EditorGUILayout.ObjectField("Alt Mesh", i.altMesh, typeof(Mesh), false);
        i.altMat = (Material)EditorGUILayout.ObjectField("Alt Material", i.altMat, typeof(Material), false);

        EditorGUILayout.Space();
        if (i.effects == null)
            i.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(i, i.effects);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").FindPropertyRelative("unlocks"), GUIContent.none);
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
