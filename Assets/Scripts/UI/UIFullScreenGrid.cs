using UnityEngine;

public class UIFullScreenGrid : MonoBehaviour {

    UIGrid grid;

    void Awake() {
        UICamera.onScreenResize += ScreenSizeChanged;
    }
    void OnDestroy() {
        UICamera.onScreenResize -= ScreenSizeChanged;
    }
    void ScreenSizeChanged() { SetCells(); }

    void Start() {
        grid = gameObject.GetComponent<UIGrid>();
        SetCells();
    }

    void SetCells() {
        UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
        float ratio = (float)mRoot.activeHeight / Screen.height;
        grid.cellWidth = Mathf.Ceil(Screen.width * ratio);
        grid.cellHeight = Mathf.Ceil(Screen.height * ratio);
        grid.Reposition();
    }
}
