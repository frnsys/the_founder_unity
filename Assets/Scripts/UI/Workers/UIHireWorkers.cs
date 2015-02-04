using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHireWorkers : UIFullScreenPager {
    private Company company;
    private GameManager gm;

    public GameObject workerItemPrefab;
    public GameObject offerPrefab;
    public GameObject noWorkersNotice;

    private List<Worker> availableWorkers = new List<Worker>();

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
    }

    public void HireWorker(Worker worker) {
        if (company.remainingSpace > 0) {
            UIOffer ic = NGUITools.AddChild(gameObject, offerPrefab).GetComponent<UIOffer>();
            ic.bodyText = "Make an offer for " + worker.name + ".";
            ic.offer = 5000;

            UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                ic.Close_();
                if (ic.offer >= worker.MinSalaryForCompany(company)) {
                    // Set the worker's salary to the offer.
                    // Reset the player offer counter.
                    // Hire the worker!
                    worker.salary = ic.offer;
                    worker.recentPlayerOffers = 0;
                    HireWorker_(worker);
                } else {
                    if (++worker.recentPlayerOffers >= 3) {
                        // If you make too many failed offers,
                        // the worker goes off the market for a bit.
                        worker.offMarketTime = 4;

                        UIManager.Instance.Alert("Your offers were too low. I've decided to take a position somewhere else.");

                        RemoveWorker(worker);
                    } else {
                        UIManager.Instance.Alert("That's way too low. I can't work for that much.");
                    }
                }
            };

            UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
                ic.Close_();
            };

            UIEventListener.Get(ic.yesButton).onClick += yesAction;
            UIEventListener.Get(ic.noButton).onClick += noAction;

        } else {
            UIManager.Instance.Alert("You don't have any space for new workers. Consider laying some people off or expanding to a new location.");
        }
    }
    private void HireWorker_(Worker worker) {
        GameManager.Instance.workerManager.HireWorker(worker);
        RemoveWorker(worker);

        // TO DO make this fancier
        UIManager.Instance.Alert("It's been my lifelong dream to work for " + company.name + "!!");
    }

    private void RemoveWorker(Worker worker) {
        int i = availableWorkers.IndexOf(worker);
        availableWorkers.Remove(worker);

        // Remove the worker item.
        GameObject workerItem = grid.gameObject.transform.GetChild(i).gameObject;
        UnityEngine.Object.DestroyImmediate(workerItem);

        // Re-wrap the grid to reset the wrapped items.
        Adjust();

        if (availableWorkers.Count == 0) {
            noWorkersNotice.SetActive(true);
        }
    }

    public void LoadWorkers(IEnumerable<Worker> workers) {
        ClearGrid();
        WorkerInsight wi = GameManager.Instance.workerInsight;
        foreach (Worker w in workers) {
            availableWorkers.Add(w);
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerItemPrefab);

            UIWorker uiw = workerItem.GetComponent<UIWorker>();
            Worker worker = w;
            UIEventListener.Get(uiw.button.gameObject).onClick += delegate(GameObject obj) {
                HireWorker(worker);
            };

            switch (wi) {
                case WorkerInsight.Basic:
                    uiw.SetBasicWorker(w);
                    break;
                case WorkerInsight.Fuzzy:
                    uiw.SetFuzzyWorker(w);
                    break;
                case WorkerInsight.Quant:
                    uiw.SetQuantWorker(w);
                    break;
            }
        }
        Adjust();
    }
}
