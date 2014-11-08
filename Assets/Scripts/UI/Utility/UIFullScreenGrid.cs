/*
 * UIFullScreenGrid
 * ================
 *
 * Sets up a grid to always have full-screen cells.
 * Will additionally handle a UIWrapContent if it
 * is also present.
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFullScreenGrid : UISimpleGrid {
    // w = top
    // x = right
    // y = bottom
    // z = left
    public Vector4 padding = Vector4.zero;

    void Awake() {
        UICamera.onScreenResize += ScreenSizeChanged;
    }
    void OnDestroy() {
        UICamera.onScreenResize -= ScreenSizeChanged;
    }
    void ScreenSizeChanged() { SetCells(); }

    void OnEnable() {
        SetCells();
    }

    public void SetCells() {
        UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);

        if (!mRoot) {
            // Default to grabbing the first
            // available UIRoot.
            mRoot = UIRoot.list[0];
        }

        float ratio = (float)mRoot.activeHeight / Screen.height;
        childSize.x = Mathf.Ceil(Screen.width * ratio) - padding.x - padding.z;
        childSize.y = Mathf.Ceil(Screen.height * ratio) - padding.w - padding.y;

        // If there is a content wrapper,
        // update that too.
        UIWrapContent contentWrap = gameObject.GetComponent<UIWrapContent>();
        if (contentWrap) {
            contentWrap.itemSize = (int)childSize.x;
        }

        Reposition();
    }

	[ContextMenu("Execute")]
    public void Reposition() {
        List<Transform> children = GetChildren(transform);

        width = (int)childSize.x;
		for (int i = 0; i < children.Count; ++i) {
			Transform child = children[i];
			Vector3 localPos = Vector3.zero;

            localPos.x = (i * (padding.x + childSize.x + padding.z)) + padding.x;
            child.localPosition = localPos;

            // Set the actual width to be full screen as well.
            child.gameObject.GetComponent<UIWidget>().width = (int)childSize.x;
        }

		EventDelegate.Execute(OnReposition);
    }
}
