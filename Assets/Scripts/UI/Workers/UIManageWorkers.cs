using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageWorkers : UIFullScreenPager {
    private Company company;
    private GameManager gm;

    public GameObject workerItemPrefab;

    private List<Worker> workers = new List<Worker>();
    private Color buttonColor = new Color(1f, 0f, 0f);
    private Color otherButtonColor = new Color(0.42f, 0.33f, 0.97f);

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

            if (gm.cloneable && !w.robot) {
                UIEventListener.Get(uiw.otherButton.gameObject).onClick += delegate(GameObject obj) {
                    CloneWorker(worker);
                };
                uiw.otherButton.gameObject.SetActive(true);
                uiw.otherButton.transform.Find("Label").GetComponent<UILabel>().text = "Clone";
                uiw.otherButton.defaultColor = otherButtonColor;
                uiw.otherButton.pressed = otherButtonColor;
                uiw.otherButton.hover = otherButtonColor;
            } else {
                uiw.otherButton.gameObject.SetActive(false);
            }
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

    public void CloneWorker(Worker worker) {
        UIManager.Instance.Confirm("Are you sure want to clone " + worker.name + "?", delegate {
                CloneWorker_(worker);
        } , null);
    }
    private void CloneWorker_(Worker worker) {
        if (worker.robot) {
            UIManager.Instance.Alert("You can't clone robots. It would violate digital copyright laws.");
            return;
        } else if (company.remainingSpace == 0 ){
            UIManager.Instance.Alert("You don't have room for anymore employees. Fire some.");
            return;
        } else {
            Worker clone = worker.Clone();
            gm.workerManager.HireWorker(clone);
        }
    }
}
