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

        target.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", target.mesh, typeof(Mesh), false);

        if (target.revenueModel == null)
            target.revenueModel = new AnimationCurve();
        target.revenueModel = EditorGUILayout.CurveField("Revenue Model", target.revenueModel, GUILayout.Height(50));

        EditorGUILayout.LabelField("Effects");
        if (target.effects == null)
            target.effects = new EffectSet();
        EffectSetRenderer.RenderEffectSet(target, target.effects);
        EditorGUILayout.Space();
    }
}
