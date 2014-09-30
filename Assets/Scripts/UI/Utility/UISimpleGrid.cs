/*
 * This is a really simple grid which automatically
 * arranges its children in rows and columns.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UISimpleGrid : UIWidget {
	public Vector2 childSize = Vector2.zero;
	public List<EventDelegate> OnReposition;

	[ContextMenu("Execute")]
    public void Reposition() {
        Bounds bounds = CalculateBounds();
        List<Transform> children = GetChildren(transform);

        int columns = Mathf.FloorToInt(bounds.size.x/childSize.x);
        int rows = Mathf.CeilToInt(children.Count/(float)columns);

        // Calculate the size the grid will have.
        int gridWidth = (int)(columns * childSize.x);
        int gridHeight = (int)(rows * childSize.y);

        // Center the grid horizontally.
        float xShift = childSize.x/2 + (bounds.size.x - gridWidth)/2;
        float yShift = childSize.y/2;

		for (int i = 0; i < children.Count; ++i) {
			Transform child = children[i];
			Vector3 localPos = Vector3.zero;

            int row = (int)Mathf.Floor(i/columns);
            int col = i % columns;

            localPos.x = (col * childSize.x) + xShift;
            localPos.y = (row * childSize.y) - yShift;

            child.localPosition = localPos;
        }

		EventDelegate.Execute(OnReposition);
    }

	public static List<Transform> GetChildren(Transform root)
	{
		List<Transform> list = new List<Transform>();

		for (int i = 0; i < root.childCount; ++i) {
			Transform trans = root.GetChild(i);

			if (trans.gameObject.activeSelf) {
				list.Add(trans);
			}
		}

		return list;
	}	
}
