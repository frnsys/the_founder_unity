using UnityEngine;
using System.Collections;

public class UIFullScreenPager : MonoBehaviour {
    public UIFullScreenGrid grid;
    public UICenterOnChild gridCenter;
    public UIScrollView scrollView;

    // The grid has to be re-wrapped whenever
    // its contents change, since UIWrapContent
    // caches the grid's contents on its start.
    private void WrapGrid() {
        NGUITools.DestroyImmediate(grid.gameObject.GetComponent<UIWrapContent>());
        UIWrapContent wrapper = grid.gameObject.AddComponent<UIWrapContent>();

        // Disable culling since it screws up the grid's layout.
        wrapper.cullContent = false;

        // The wrapper's item width is the same as the grid's cell width, duh
        wrapper.itemSize = (int)grid.childSize.x;
    }

    public void Adjust() {
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    public void ClearGrid() {
        int children = grid.transform.childCount;
        for (int i=0;i<children;i++) {
            Debug.Log("Destroying...");
            NGUITools.Destroy(grid.transform.GetChild(0).gameObject);
        }
    }
}
