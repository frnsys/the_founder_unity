using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// This is a hack but I don't feel like fighting with Unity's editor stuff.
public class EffectSetRenderer {
    static int t_i = 0;
    static List<Type> types = new List<Type> {
        typeof(WorkerEffect),
        typeof(CashEffect),
        typeof(ProductEffect)
    };
    static string[] type_names = types.Select(t => t.ToString()).ToArray();

    public static EffectSet RenderEffectSet(UnityEngine.Object target, EffectSet es) {
        for (int i=0; i < es.effects.Count; i++) {
            EditorGUILayout.Space();
            es.effects[i] = RenderEffect(es.effects[i]);
            if (GUILayout.Button("Delete")) {
                es.effects.Remove(es.effects[i]);
            }
        }

        EditorGUILayout.BeginHorizontal();
        t_i = EditorGUILayout.Popup(t_i, type_names);
        if (GUILayout.Button("Add New Effect")) {
            es.effects.Add((IEffect)Activator.CreateInstance(types[t_i]));
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.EndHorizontal();
        return es;
    }

    private static IEffect RenderEffect(IEffect ef) {
        EditorGUILayout.LabelField(ef.GetType().ToString());
        string[] stats;
        int name_i;
        switch (ef.GetType().ToString()) {
            case "WorkerEffect":
                WorkerEffect w_ef = (WorkerEffect)ef;
                if (w_ef.buff == null)
                    w_ef.buff = new StatBuff("Happiness", 0);
                stats = new string[] {
                    "Happiness",
                    "Productivity",
                    "Charisma",
                    "Cleverness",
                    "Creativity"
                };
                EditorGUILayout.BeginHorizontal();
                name_i = EditorGUILayout.Popup(Array.IndexOf(stats, w_ef.buff.name), stats);
                w_ef.buff.name = stats[name_i];
                w_ef.buff.value =  EditorGUILayout.FloatField(w_ef.buff.value);
                EditorGUILayout.EndHorizontal();
                return w_ef;

            case "CashEffect":
                CashEffect cash_ef = (CashEffect)ef;
                cash_ef.cash =  EditorGUILayout.FloatField(cash_ef.cash);
                return cash_ef;

            case "ResearchEffect":
                ResearchEffect r_ef = (ResearchEffect)ef;
                if (r_ef.buff == null)
                    r_ef.buff = new StatBuff("Research", 0);
                r_ef.buff.value = EditorGUILayout.FloatField(r_ef.buff.value);
                return r_ef;

            case "OpinionEffect":
                OpinionEffect oe_ef = (OpinionEffect)ef;
                if (oe_ef.opinionEvent == null)
                    oe_ef.opinionEvent = new OpinionEvent();
                oe_ef.opinionEvent.opinion.value = EditorGUILayout.FloatField("Opinion", oe_ef.opinionEvent.opinion.value);
                oe_ef.opinionEvent.publicity.value = EditorGUILayout.FloatField("Publicity", oe_ef.opinionEvent.publicity.value);
                return oe_ef;

            case "EventEffect":
                EventEffect e_ef = (EventEffect)ef;
                e_ef.delay = EditorGUILayout.FloatField("Delay", e_ef.delay);
                e_ef.probability = EditorGUILayout.FloatField("Probability", e_ef.probability);
                e_ef.gameEvent = (GameEvent)EditorGUILayout.ObjectField(e_ef.gameEvent, typeof(GameEvent));
                return e_ef;

            case "ProductEffect":
                ProductEffect pef = (ProductEffect)ef;

                // Product Types
                EditorGUILayout.LabelField("Product Types");
                for (int i=0; i < pef.productTypes.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    pef.productTypes[i] = (ProductType)EditorGUILayout.ObjectField(pef.productTypes[i], typeof(ProductType));
                    if (GUILayout.Button("Delete")) {
                        pef.productTypes.Remove(pef.productTypes[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                ProductType newProductType = (ProductType)EditorGUILayout.ObjectField(null, typeof(ProductType));
                if (newProductType != null)
                    pef.productTypes.Add(newProductType);

                // Verticals
                EditorGUILayout.LabelField("Verticals");
                for (int i=0; i < pef.verticals.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    pef.verticals[i] = (Vertical)EditorGUILayout.ObjectField(pef.verticals[i], typeof(Vertical));
                    if (GUILayout.Button("Delete")) {
                        pef.verticals.Remove(pef.verticals[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                Vertical newVertical = (Vertical)EditorGUILayout.ObjectField(null, typeof(Vertical));
                if (newVertical != null)
                    pef.verticals.Add(newVertical);

                if (pef.buff == null)
                    pef.buff = new StatBuff("Design", 0);
                stats = new string[] {
                    "Design",
                    "Marketing",
                    "Engineering"
                };
                EditorGUILayout.BeginHorizontal();
                name_i = EditorGUILayout.Popup(Array.IndexOf(stats, pef.buff.name), stats);
                pef.buff.name = stats[name_i];
                pef.buff.value =  EditorGUILayout.FloatField(pef.buff.value);
                EditorGUILayout.EndHorizontal();

                return pef;

            default:
                break;
        }
        return ef;
    }
}
