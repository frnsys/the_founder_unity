using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(ProductEffect))]
class ProductEffectDrawer : SuperPropertyDrawer {
    // The names of the list properties.
    private string[] listPropertyNames = new string[] {
        "industries",
        "markets",
        "productTypes",
        "buffs"
    };

    public override Rect Edit(Rect position, SerializedProperty property) {
        foreach (string key in listPropertyNames) {
            position = DrawReorderableList(position, property, property.FindPropertyRelative(key));
        }
        return position;
    }
}