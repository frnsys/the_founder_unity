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

        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"));
        EditorGUILayout.Space();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
