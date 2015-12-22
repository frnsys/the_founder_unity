using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// This is a hack but I don't feel like fighting with Unity's editor stuff.
public class EffectSetRenderer {

    private static string[] workerStats = new string[] {
        "Happiness",
        "Productivity",
        "Charisma",
        "Cleverness",
        "Creativity"
    };

    private static string[] productStats = new string[] {
        "Design",
        "Marketing",
        "Engineering"
    };

    public static EffectSet RenderEffectSet(UnityEngine.Object target, EffectSet es) {
        es.cash =  EditorGUILayout.FloatField("Cash Effect", es.cash);
        es.opinion =  EditorGUILayout.FloatField("Opinion Effect", es.opinion);
        es.hype =  EditorGUILayout.IntField("Hype Effect", es.hype);
        es.forgettingRate = EditorGUILayout.FloatField("Forgetting Rate (+/-)", es.forgettingRate);
        es.spendingMultiplier = EditorGUILayout.FloatField("Spending Multiplier (+/-)", es.spendingMultiplier);
        es.wageMultiplier = EditorGUILayout.FloatField("Wage Multiplier (+/-)", es.wageMultiplier);
        es.economicStability = EditorGUILayout.FloatField("Economic Stability (+/-)", es.economicStability);
        es.taxRate = EditorGUILayout.FloatField("Tax Rate (+/-)", es.taxRate);
        es.expansionCostMultiplier = EditorGUILayout.FloatField("Expansion Cost (+/-)", es.expansionCostMultiplier);
        es.specialEffect = (EffectSet.Special)EditorGUILayout.EnumPopup("Special Effect", es.specialEffect);
        es.costMultiplier = EditorGUILayout.FloatField("Cost Multiplier (+/-)", es.costMultiplier);

        EditorGUILayout.LabelField("Awakens AI Company");
        es.aiCompany = (AICompany)EditorGUILayout.ObjectField(es.aiCompany, typeof(AICompany), false);

        EditorGUILayout.LabelField("Game Event");
        es.gameEvent = (GameEvent)EditorGUILayout.ObjectField(es.gameEvent, typeof(GameEvent), false);
        if (es.gameEvent != null) {
            EditorGUILayout.BeginHorizontal();
            es.eventProbability = EditorGUILayout.FloatField("Probability (0-1)", es.eventProbability);
            if (GUILayout.Button("Delete")) {
                es.gameEvent = null;
                es.eventProbability = 0;
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Worker Effects");
        if (es.workerEffects != null) {
            for (int i=0; i < es.workerEffects.Count; i++) {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                StatBuff we = es.workerEffects[i];
                int name_i = EditorGUILayout.Popup(Array.IndexOf(workerStats, we.name), workerStats);
                we.name = workerStats[name_i];
                we.value =  EditorGUILayout.FloatField(we.value);

                if (GUILayout.Button("Delete")) {
                    es.workerEffects.Remove(we);
                    if (es.workerEffects.Count == 0)
                        es.workerEffects = null;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Add New Worker Effect")) {
            if (es.workerEffects == null)
                es.workerEffects = new List<StatBuff>();
            es.workerEffects.Add(new StatBuff("Happiness", 0));
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.LabelField("Product Effects");
        if (es.productEffects != null) {
            for (int i=0; i < es.productEffects.Count; i++) {
                ProductEffect pef = es.productEffects[i];

                // Product Types
                EditorGUILayout.LabelField("Product Types");
                for (int j=0; j < pef.productTypes.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    pef.productTypes[j] = (ProductType)EditorGUILayout.ObjectField(pef.productTypes[j], typeof(ProductType), false);
                    if (GUILayout.Button("Delete")) {
                        pef.productTypes.Remove(pef.productTypes[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                ProductType newProductType = (ProductType)EditorGUILayout.ObjectField(null, typeof(ProductType), false);
                if (newProductType != null)
                    pef.productTypes.Add(newProductType);

                // Verticals
                EditorGUILayout.LabelField("Verticals");
                for (int j=0; j < pef.verticals.Count; j++) {
                    EditorGUILayout.BeginHorizontal();
                    pef.verticals[j] = (Vertical)EditorGUILayout.ObjectField(pef.verticals[j], typeof(Vertical), false);
                    if (GUILayout.Button("Delete")) {
                        pef.verticals.Remove(pef.verticals[j]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                Vertical newVertical = (Vertical)EditorGUILayout.ObjectField(null, typeof(Vertical), false);
                if (newVertical != null)
                    pef.verticals.Add(newVertical);

                if (pef.buff == null)
                    pef.buff = new StatBuff("Design", 0);

                EditorGUILayout.BeginHorizontal();
                int name_j = EditorGUILayout.Popup(Array.IndexOf(productStats, pef.buff.name), productStats);
                pef.buff.name = productStats[name_j];
                pef.buff.value =  EditorGUILayout.FloatField(pef.buff.value);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Delete")) {
                    es.productEffects.Remove(pef);
                    if (es.productEffects.Count == 0)
                        es.productEffects = null;
                }
            }
        }
        if (GUILayout.Button("Add New Product Effect")) {
            if (es.productEffects == null)
                es.productEffects = new List<ProductEffect>();
            es.productEffects.Add(new ProductEffect("Design"));
            EditorUtility.SetDirty(target);
        }

        return es;
    }
}
