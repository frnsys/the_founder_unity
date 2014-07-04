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
        grid.cellWidth = Screen.width;
        grid.cellHeight = Screen.height;
        grid.Reposition();
    }
}
