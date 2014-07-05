using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(ProductRecipe))]
internal class ProductRecipeInspector : Editor {

    ProductRecipe p;

    List<ProductType> productTypes;
    string[] pt_o;

    List<Industry> industries;
    string[] i_o;

    List<Market> markets;
    string[] m_o;

    void OnEnable() {
        p = target as ProductRecipe;

        productTypes = ProductType.LoadAll();
        pt_o = productTypes.Select(i => i.ToString()).ToArray();

        industries = Industry.LoadAll();
        i_o = industries.Select(i => i.ToString()).ToArray();

        markets = Market.LoadAll();
        m_o = markets.Select(i => i.ToString()).ToArray();


        // Defaults
        if (p.productType == null)
            p.productType = productTypes[0];

        if (p.industry == null)
            p.industry = industries[0];

        if (p.market == null)
            p.market = markets[0];
    }

    public override void OnInspectorGUI() {

        int pt_i = EditorGUILayout.Popup(Array.IndexOf(pt_o, p.productType.ToString()), pt_o);
        p.productType = productTypes[pt_i];

        int i_i = EditorGUILayout.Popup(Array.IndexOf(i_o, p.industry.ToString()), i_o);
        p.industry = industries[i_i];

        int m_i = EditorGUILayout.Popup(Array.IndexOf(m_o, p.market.ToString()), m_o);
        p.market = markets[m_i];

        EditorGUILayout.Space();

        p.appeal_W = EditorGUILayout.FloatField("Appeal Weight", p.appeal_W);
        p.usability_W = EditorGUILayout.FloatField("Usability Weight", p.usability_W);
        p.performance_W = EditorGUILayout.FloatField("Performance Weight", p.performance_W);
        EditorGUILayout.Space();

        p.appeal_I = EditorGUILayout.FloatField("Appeal Ideal", p.appeal_I);
        p.usability_I = EditorGUILayout.FloatField("Usability Ideal", p.usability_I);
        p.performance_I = EditorGUILayout.FloatField("Performance Ideal", p.performance_I);
        EditorGUILayout.Space();

        p.progressRequired = EditorGUILayout.FloatField("Progress Required", p.progressRequired);
        p.maxLongevity = EditorGUILayout.FloatField("Max Longevity", p.maxLongevity);
        p.maxRevenue = EditorGUILayout.FloatField("Max Revenue", p.maxRevenue);
        p.maintenance = EditorGUILayout.FloatField("Maintenance Cost", p.maintenance);
        EditorGUILayout.Space();


        // Outcome management.
        EditorGUILayout.LabelField("Outcomes");
        EditorGUI.indentLevel = 1;
        for (int i=0; i < p.outcomes.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            p.outcomes[i] = EditorGUILayout.TextField(p.outcomes[i]);
            if (GUILayout.Button("Delete")) {
                p.outcomes.Remove(p.outcomes[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Outcome")) {
            p.outcomes.Add("New outcome");
        }
        EditorGUI.indentLevel = 0;


        // Let Unity know to save on changes.
        if (GUI.changed) {
            EditorUtility.SetDirty(target);

            // Update asset filename.
            string path = AssetDatabase.GetAssetPath(target);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != p.ToString()) {
                AssetDatabase.RenameAsset(path, p.ToString());
                AssetDatabase.Refresh();
            }
        }
    }

}
