using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// This is a hack but I don't feel like fighting with Unity's editor stuff.
public class ConditionsRenderer {
    static int t_i = 0;
    static List<Type> types = new List<Type> {
        typeof(PublicityCondition)
    };
    static string[] type_names = types.Select(t => t.ToString()).ToArray();

    public static List<Condition> RenderConditions(UnityEngine.Object target, List<Condition> cs) {
        for (int i=0; i < cs.Count; i++) {
            EditorGUILayout.Space();
            cs[i] = RenderCondition(cs[i]);
            if (GUILayout.Button("Delete")) {
                cs.Remove(cs[i]);
            }
        }

        EditorGUILayout.BeginHorizontal();
        t_i = EditorGUILayout.Popup(t_i, type_names);
        if (GUILayout.Button("Add New Condition")) {
            cs.Add((Condition)Activator.CreateInstance(types[t_i]));
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.EndHorizontal();
        return cs;
    }

    private static Condition RenderCondition(Condition c) {
        EditorGUILayout.LabelField(c.GetType().ToString());
        string[] stats;
        int name_i;
        switch (c.GetType().ToString()) {
            case "PublicityCondition":
                PublicityCondition pub_c = (PublicityCondition)c;
                EditorGUILayout.BeginHorizontal();
                pub_c.value =  EditorGUILayout.FloatField("Publicity", pub_c.value);
                EditorGUILayout.EndHorizontal();
                return pub_c;

            default:
                break;
        }
        return c;
    }
}
