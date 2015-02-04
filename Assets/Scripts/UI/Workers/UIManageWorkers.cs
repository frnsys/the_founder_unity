using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageWorkers : UIFullScreenPager {
    private Company company;
    private GameManager gm;

    public GameObject workerItemPrefab;

    private List<Worker> workers = new List<Worker>();
    private Color buttonColor = new Color(1f, 0f, 0f);

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        LoadWorkers();
    }

    private void LoadWorkers() {
        ClearGrid();
        foreach (Worker w in company.workers) {
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerItemPrefab);
            UIWorker uiw = workerItem.GetComponent<UIWorker>();
            uiw.worker = w;

            workers.Add(w);

            // Setup the fire button.
            Worker worker = w;
            UIEventListener.Get(uiw.button.gameObject).onClick += delegate(GameObject obj) {
                FireWorker(worker);
            };
            uiw.button.transform.Find("Label").GetComponent<UILabel>().text = "Let Go";
            uiw.button.defaultColor = buttonColor;
            uiw.button.pressed = buttonColor;
            uiw.button.hover = buttonColor;
        }
        Adjust();
    }

    public void FireWorker(Worker worker) {
        UIManager.Instance.Confirm("Are you sure want to fire " + worker.name + "?", delegate {
                FireWorker_(worker);
        } , null);
    }
    private void FireWorker_(Worker worker) {
        GameManager.Instance.playerCompany.FireWorker(worker);

        int i = workers.IndexOf(worker);
        workers.Remove(worker);

        // Remove the worker item.
        GameObject workerItem = grid.gameObject.transform.GetChild(i).gameObject;
        UnityEngine.Object.DestroyImmediate(workerItem);

        // Re-wrap the grid to reset the wrapped items.
        Adjust();

        // TO DO make this fancier
        UIManager.Instance.Alert("Your company is shit!!!1!");
    }
}
