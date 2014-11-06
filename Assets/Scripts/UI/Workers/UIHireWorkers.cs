using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHireWorkers : MonoBehaviour {
    public GameObject workerItemPrefab;
    public UIFullScreenGrid grid;
    public UICenterOnChild gridCenter;
    public UIScrollView scrollView;

    public UIButton hireButton;

    private Worker currentWorker;
    private List<Worker> availableWorkers = new List<Worker>();

    void OnEnable() {
        gridCenter.onFinished = OnCenter;

        LoadWorkers();
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the buttons
            // while dragging.
            hireButton.isEnabled = false;
        }
    }

    public void HireWorker() {
        UIManager.Instance.Confirm("Are you sure want to hire " + currentWorker.name + "?", HireWorker_, null);
    }
    private void HireWorker_() {
        GameManager.Instance.playerCompany.HireWorker(currentWorker);

        int i = availableWorkers.IndexOf(currentWorker);
        availableWorkers.Remove(currentWorker);

        // Remove the worker item.
        GameObject workerItem = grid.gameObject.transform.GetChild(i).gameObject;
        UnityEngine.Object.DestroyImmediate(workerItem);

        // Re-wrap the grid to reset the wrapped items.
        WrapGrid();

        // TO DO make this fancier
        UIManager.Instance.Alert("It's been my lifelong dream to work for " + GameManager.Instance.playerCompany.name + "!!");
    }

    private void OnCenter() {
        // Re-enable the buttons
        // when centering the item after a swipe is complete.
        hireButton.isEnabled = true;
        currentWorker = gridCenter.centeredObject.GetComponent<UIWorker>().worker as Worker;
    }


    private void LoadWorkers() {
        foreach (Worker w in GameManager.Instance.availableWorkers) {
            availableWorkers.Add(w);
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerItemPrefab);
            workerItem.GetComponent<UIWorker>().worker = w;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

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
}
