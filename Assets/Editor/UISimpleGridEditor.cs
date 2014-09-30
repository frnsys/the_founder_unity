using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISimpleGrid), true)]
public class UISimpleGridEditor : UIWidgetInspector
{
	public override void OnInspectorGUI()
	{
		UISimpleGrid grid = target as UISimpleGrid;

		serializedObject.Update();

        NGUIEditorTools.DrawProperty("Width", serializedObject, "childSize.x", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Height", serializedObject, "childSize.y", GUILayout.MinWidth(20f));

		if (serializedObject.ApplyModifiedProperties())
			grid.Reposition();

		base.OnInspectorGUI();
	}
}
