/*
 * This class dynamically assigns itself a scroll view to control
 * based on what scroll view is currently centered in the associated grid.
 * It looks for children named "Content Scroll".
 */

using UnityEngine;
using System.Collections;

public class UIDynamicDragScrollView : UIDragScrollView {

    public UICenterOnChild gridCenter;
    private UIDragScrollView subDragger;

    public virtual void Start() {
        gridCenter.onFinished = OnCenter;
        scrollView = gridCenter.centeredObject.transform.Find("Content Scroll").GetComponent<UIScrollView>();
    }

    private void OnCenter() {
        scrollView = gridCenter.centeredObject.transform.Find("Content Scroll").GetComponent<UIScrollView>();
    }
}
