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
        switch (ef.GetType().ToString()) {

            case "WorkerEffect":
                WorkerEffect w_ef = (WorkerEffect)ef;
                if (w_ef.buff == null)
                    w_ef.buff = new StatBuff("Happiness", 0);
                string[] stats = new string[] {
                    "Happiness",
                    "Productivity",
                    "Charisma",
                    "Cleverness",
                    "Creativity"
                };
                EditorGUILayout.BeginHorizontal();
                int name_i = EditorGUILayout.Popup(Array.IndexOf(stats, w_ef.buff.name), stats);
                w_ef.buff.name = stats[name_i];
                w_ef.buff.value =  EditorGUILayout.FloatField(w_ef.buff.value);
                EditorGUILayout.EndHorizontal();
                return w_ef;

            case "CashEffect":
                CashEffect cash_ef = (CashEffect)ef;
                cash_ef.cash =  EditorGUILayout.FloatField(cash_ef.cash);
                return cash_ef;

            default:
                break;
        }
        return ef;
    }
}
