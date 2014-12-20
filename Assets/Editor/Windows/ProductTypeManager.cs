using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductTypeManager : ManagerWindow<ProductType> {
    [MenuItem("The Founder/Product Type Manager")]
    static void Init() {
        ProductTypeManager window = EditorWindow.CreateInstance<ProductTypeManager>();
        window.targets = ProductType.LoadAll();
        window.Show();
    }

    protected override string path {
        get { return "Assets/Resources/Products/Types"; }
    }

    protected override void DrawInspector() {
        EditorStyles.textField.wordWrap = true;

        EditorGUILayout.LabelField("Product Type");
        target.name = EditorGUILayout.TextField("Name", target.name);
        target.description = EditorGUILayout.TextArea(target.description, GUILayout.Height(50));

        EditorGUILayout.LabelField("Required Verticals");
        for (int i=0; i < target.requiredVerticals.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            target.requiredVerticals[i] = (Vertical)EditorGUILayout.ObjectField(target.requiredVerticals[i], typeof(Vertical));
            if (GUILayout.Button("Delete")) {
                target.requiredVerticals.Remove(target.requiredVerticals[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Required Vertical")) {
            target.requiredVerticals.Add(null);
        }

        EditorGUILayout.LabelField("Required Infrastructure");
        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            target.requiredInfrastructure[t] = EditorGUILayout.IntField(t.ToString(), target.requiredInfrastructure[t]);
        }
    }
}
