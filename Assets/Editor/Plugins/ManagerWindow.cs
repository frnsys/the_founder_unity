using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class ManagerWindow<T> : EditorWindow where T : ScriptableObject {
    protected List<T> targets = new List<T>();
    protected int selIdx = 0;
    protected string[] targetNames;
    protected SerializedObject serializedObject;
    protected T target;
    protected string typeName = typeof(T).FullName;
    private Vector2 scrollPos = Vector2.zero;

    protected abstract string path { get; }
    protected abstract void DrawInspector();

    void OnGUI() {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Add New " + typeName)) {
            T newTarget = ScriptableObject.CreateInstance<T>();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeName + ".asset");
            AssetDatabase.CreateAsset(newTarget, assetPathAndName);
            AssetDatabase.SaveAssets();
            targets.Add(newTarget);
            selIdx = targets.Count - 1;
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        targetNames = targets.Select(i => i.name).ToArray();
        EditorGUILayout.Space();
        selIdx = EditorGUILayout.Popup(Array.IndexOf(targetNames, targets[selIdx].name), targetNames);
        EditorGUILayout.Space();

        target = targets[selIdx];
        serializedObject = new SerializedObject(target);
        serializedObject.Update();

        // The "Inspector". Have to recreate it because we can't embed an inspector :(
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.Width(this.position.width), GUILayout.Height(this.position.height - 100));
        DrawInspector();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != target.name) {
                AssetDatabase.RenameAsset(path, target.name);
                AssetDatabase.Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Delete " + target.name)) {
            targets.Remove(target);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(target));
            AssetDatabase.Refresh();
        }

        EditorGUILayout.EndScrollView();
    }
}
