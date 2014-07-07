using UnityEngine;

public class UIFullScreenGrid : MonoBehaviour {

    public UIGrid grid;

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
        grid.cellWidth = Mathf.Ceil(Screen.width * ratio);
        grid.cellHeight = Mathf.Ceil(Screen.height * ratio);

        // If there is a content wrapper,
        // update that too.
        UIWrapContent contentWrap = grid.gameObject.GetComponent<UIWrapContent>();
        if (contentWrap) {
            contentWrap.itemSize = (int)grid.cellWidth;
        }

        grid.Reposition();
    }
}
