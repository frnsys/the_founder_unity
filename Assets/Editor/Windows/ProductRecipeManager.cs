using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductRecipeManager : ManagerWindow<ProductRecipe> {
    [MenuItem("The Founder/Product Recipe Manager")]
    static void Init() {
        ProductRecipeManager window = EditorWindow.CreateInstance<ProductRecipeManager>();
        window.targets = ProductRecipe.LoadAll();
        window.Show();
    }

    protected override string path {
        get { return "Assets/Resources/Products/Recipes"; }
    }

    List<ProductType> productTypes;
    string[] pt_o;
    protected override void DrawInspector() {
        productTypes = ProductType.LoadAll();
        pt_o = productTypes.Select(i => i.ToString()).ToArray();

        EditorGUILayout.LabelField("ProductTypes");
        EditorGUI.indentLevel = 1;
        for (int i=0; i < target.productTypes.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            int pt_i = EditorGUILayout.Popup(Array.IndexOf(pt_o, target.productTypes[i].ToString()), pt_o);
            target.productTypes[i] = productTypes[pt_i];
            if (GUILayout.Button("Delete")) {
                target.productTypes.Remove(target.productTypes[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Maximum two product types for a product.
        if (target.productTypes.Count <= 2) {
            if (GUILayout.Button("Add New Product Type")) {
                target.productTypes.Add(productTypes[0]);
            }
        }
        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Names (comma-delimited)");
        target.names = EditorGUILayout.TextArea(target.names);

        target.design_W = EditorGUILayout.FloatField("Design Weight", target.design_W);
        target.marketing_W = EditorGUILayout.FloatField("Marketing Weight", target.marketing_W);
        target.engineering_W = EditorGUILayout.FloatField("Engineering Weight", target.engineering_W);
        EditorGUILayout.Space();

        target.design_I = EditorGUILayout.FloatField("Design Ideal", target.design_I);
        target.marketing_I = EditorGUILayout.FloatField("Marketing Ideal", target.marketing_I);
        target.engineering_I = EditorGUILayout.FloatField("Engineering Ideal", target.engineering_I);
        EditorGUILayout.Space();

        target.maxLongevity = EditorGUILayout.FloatField("Max Longevity", target.maxLongevity);
        target.maxRevenue = EditorGUILayout.FloatField("Max Revenue", target.maxRevenue);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Effects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUIContent.none);
    }
}
