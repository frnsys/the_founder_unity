/*
 * This is a really simple grid which automatically
 * horizontally centers its children. It can figure out the child cell size as well.
 * For this to work you need to set the UIWidget's pivots to horizontal center and vertical top.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UICenteredGrid : UIWidget {
	public List<EventDelegate> OnReposition;
    public bool fullWidth = false;

	[ContextMenu("Execute")]
    public void Reposition() {
        Bounds bounds = CalculateBounds();
        List<Transform> children = GetChildren(transform);

		for (int i = 0; i < children.Count; ++i) {
			Transform child = children[i];
			Vector3 localPos = Vector3.zero;

            if (fullWidth)
                child.GetComponent<UIWidget>().width = width;

            Vector3 size = child.GetComponent<UIWidget>().CalculateBounds().size;

            localPos.x = 0;
            localPos.y = -(i * size.y) - size.y/2f;

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
