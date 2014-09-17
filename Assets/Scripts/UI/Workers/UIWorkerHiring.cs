/*
 * Worker Hiring
 * ===================
 *
 * UI for hiring workers.
 *
 */

using UnityEngine;
using System.Collections;

public class UIWorkerHiring : MonoBehaviour {
    private GameManager gm;

    public UIGrid grid;
    public UICenterOnChild gridCenter;
    public UIScrollView scrollView;
    public GameObject workerPrefab;
    public UIButton selectButton;

    private Worker currentWorker;

    void OnEnable() {
        gm = GameManager.Instance;
        gridCenter.onFinished = OnCenter;

        LoadWorkers();
    }

    private void OnCenter() {
        // Re-enable the select button
        // when centering the item after a swipe is complete.
        selectButton.isEnabled = true;
        currentWorker = gridCenter.centeredObject.GetComponent<UIWorker>().worker as Worker;
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the select button
            // while dragging.
            selectButton.isEnabled = false;
        }
    }

    public void HireWorker() {
        // Disable the button.
        //button.isEnabled = false;
        // this should just remove the worker from the available workers.

        Debug.Log("I WAS CLICKED");
        GameManager.Instance.HireWorker(currentWorker);
    }

    // NOT USED
    public void UpdateAvailableWorkers() {
        // Clear the grid.
        while (grid.transform.childCount > 0)
            NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject);

        foreach (Worker worker in gm.unlocked.workers) {
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerPrefab);
            workerItem.GetComponent<UIWorker>().worker = worker;
            UIButton hireButton = workerItem.transform.FindChild("Hire Button").GetComponent<UIButton>();

            // Add the OnClick event for each hire button.
            EventDelegate e = new EventDelegate(this, "HireWorker");
            e.oneShot = true;
            e.parameters[0] = new EventDelegate.Parameter(hireButton, "");
            e.parameters[1] = new EventDelegate.Parameter(workerItem.GetComponent<UIWorker>(), "worker");
            hireButton.onClick.Add(e);
        }

        grid.Reposition();
    }

    private void LoadWorkers() {
        ClearGrid();
        foreach (Worker worker in gm.unlocked.workers) {
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerPrefab);
            workerItem.GetComponent<UIWorker>().worker = worker;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void ClearGrid() {
        while (grid.transform.childCount > 0)
            NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject);
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
        wrapper.itemSize = (int)grid.cellWidth;
    }
}


