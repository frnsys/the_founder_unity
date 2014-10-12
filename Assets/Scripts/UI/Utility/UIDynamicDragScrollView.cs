/*
 * This class dynamically assigns itself a scroll view to control
 * based on what scroll view is currently centered in the associated grid.
 * It looks for children named "Content Scroll".
 */

using UnityEngine;
using System.Collections;

public class UIDynamicDragScrollView : UIDragScrollView {

    public UICenterOnChild gridCenter;

    public virtual void Start() {
        gridCenter.onFinished = OnCenter;

        // On Start there may not be any objects in the grid yet.
        if (gridCenter.centeredObject) {
            scrollView = gridCenter.centeredObject.transform.Find("Content Scroll").GetComponent<UIScrollView>();
        }
    }

    private void OnCenter() {
        scrollView = gridCenter.centeredObject.transform.Find("Content Scroll").GetComponent<UIScrollView>();
    }
}
