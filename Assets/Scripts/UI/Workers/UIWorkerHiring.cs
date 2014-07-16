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
    public GameObject workerPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        UpdateAvailableWorkers();
    }

    public void HireWorker(UIButton button, Worker worker) {
        // Disable the button.
        button.isEnabled = false;

        GameManager.Instance.HireWorker(worker);
    }

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
}


