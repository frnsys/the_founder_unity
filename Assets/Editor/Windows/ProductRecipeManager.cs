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
        window.Show();
    }

    protected override string path {
        get { return "Assets/Resources/Products/Recipes"; }
    }
    protected override List<ProductRecipe> LoadTargets() {
        return ProductRecipe.LoadAll();
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

        target.appeal_W = EditorGUILayout.FloatField("Appeal Weight", target.appeal_W);
        target.usability_W = EditorGUILayout.FloatField("Usability Weight", target.usability_W);
        target.performance_W = EditorGUILayout.FloatField("Performance Weight", target.performance_W);
        EditorGUILayout.Space();

        target.appeal_I = EditorGUILayout.FloatField("Appeal Ideal", target.appeal_I);
        target.usability_I = EditorGUILayout.FloatField("Usability Ideal", target.usability_I);
        target.performance_I = EditorGUILayout.FloatField("Performance Ideal", target.performance_I);
        EditorGUILayout.Space();

        target.progressRequired = EditorGUILayout.FloatField("Progress Required", target.progressRequired);
        target.maxLongevity = EditorGUILayout.FloatField("Max Longevity", target.maxLongevity);
        target.maxRevenue = EditorGUILayout.FloatField("Max Revenue", target.maxRevenue);
        target.maintenance = EditorGUILayout.FloatField("Maintenance Cost", target.maintenance);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Effects");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUIContent.none);
    }
}
