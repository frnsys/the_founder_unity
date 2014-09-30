using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIFullScreenGrid), true)]
public class UIFullScreenGridEditor : UISimpleGridEditor
{
	public override void OnInspectorGUI()
	{
		UIFullScreenGrid grid = target as UIFullScreenGrid;

		serializedObject.Update();

        NGUIEditorTools.DrawProperty("Top", serializedObject, "padding.w", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Right", serializedObject, "padding.x", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Bottom", serializedObject, "padding.y", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Left", serializedObject, "padding.z", GUILayout.MinWidth(20f));

		if (serializedObject.ApplyModifiedProperties())
			grid.Reposition();

		base.OnInspectorGUI();
	}
}
