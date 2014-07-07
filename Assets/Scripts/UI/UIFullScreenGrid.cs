using UnityEngine;

public class UIFullScreenGrid : MonoBehaviour {

    public UIGrid grid;
    public UIWrapContent contentWrap;

    void Awake() {
        UICamera.onScreenResize += ScreenSizeChanged;
    }
    void OnDestroy() {
        UICamera.onScreenResize -= ScreenSizeChanged;
    }
    void ScreenSizeChanged() { SetCells(); }

    void Start() {
        SetCells();
    }

    void SetCells() {
        UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
        float ratio = (float)mRoot.activeHeight / Screen.height;
        grid.cellWidth = Mathf.Ceil(Screen.width * ratio);
        grid.cellHeight = Mathf.Ceil(Screen.height * ratio);

        // If there is a content wrapper,
        // update that too.
        if (contentWrap) {
            contentWrap.itemSize = (int)grid.cellWidth;
        }

        grid.Reposition();
    }
}
