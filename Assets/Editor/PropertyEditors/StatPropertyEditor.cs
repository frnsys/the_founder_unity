using FullInspector;
using UnityEditor;
using UnityEngine;

// Exposes only a Stat's `baseValue` for editing.
[CustomPropertyEditor(typeof(Stat))]
public class StatPropertyEditor : PropertyEditor<Stat> {
    public override Stat Edit(Rect region, GUIContent label, Stat element) {
        element.baseValue = EditorGUI.FloatField(region, label, element.baseValue);
        return element;
    }
    public override float GetElementHeight(GUIContent label, Stat element) {
        return EditorStyles.numberField.CalcHeight(label, 1000);
    }
}
