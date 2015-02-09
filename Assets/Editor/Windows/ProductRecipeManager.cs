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

    protected override void DrawInspector() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("productTypes"), true);

        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.LabelField("Names (comma-delimited)");
        target.names = EditorGUILayout.TextArea(target.names);

        EditorGUILayout.LabelField("Description");
        target.description = EditorGUILayout.TextArea(target.description, GUILayout.Height(50));
        EditorGUILayout.Space();

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
        if (target.effects == null)
            target.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(target, target.effects);
        EditorGUILayout.Space();
    }
}
