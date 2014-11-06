using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageWorkers : MonoBehaviour {
    public GameObject workerItemPrefab;
    public UIFullScreenGrid grid;
    public UICenterOnChild gridCenter;
    public UIScrollView scrollView;

    public UIButton fireButton;
    public UIButton upgradeButton;

    private Worker currentWorker;
    private List<Worker> workers = new List<Worker>();

    void OnEnable() {
        gridCenter.onFinished = OnCenter;

        LoadWorkers();
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the buttons
            // while dragging.
            fireButton.isEnabled = false;
            upgradeButton.isEnabled = false;
        }
    }

    private void OnCenter() {
        // Re-enable the buttons
        // when centering the item after a swipe is complete.
        fireButton.isEnabled = true;
        upgradeButton.isEnabled = true;
        currentWorker = gridCenter.centeredObject.GetComponent<UIWorker>().worker as Worker;
    }


    public void FireWorker() {
        UIManager.Instance.Confirm("Are you sure want to fire " + currentWorker.name + "?", FireWorker_, null);
    }
    private void FireWorker_() {
        GameManager.Instance.playerCompany.FireWorker(currentWorker);

        int i = workers.IndexOf(currentWorker);
        workers.Remove(currentWorker);

        // Remove the worker item.
        GameObject workerItem = grid.gameObject.transform.GetChild(i).gameObject;
        UnityEngine.Object.DestroyImmediate(workerItem);

        // Re-wrap the grid to reset the wrapped items.
        WrapGrid();

        // TO DO make this fancier
        UIManager.Instance.Alert("Your company is shit!!!1!");
    }

    public void UpgradeWorker() {
        UIManager.Instance.Confirm("Are you sure want to promote " + currentWorker.name + "?", UpgradeWorker_, null);
    }
    private void UpgradeWorker_() {
        // TO DO Upgrade worker
    }

    private void LoadWorkers() {
        foreach (Worker w in GameManager.Instance.playerCompany.workers) {
            workers.Add(w);
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
