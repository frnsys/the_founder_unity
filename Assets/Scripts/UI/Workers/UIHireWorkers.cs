using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHireWorkers : UIFullScreenPager {
    public GameObject workerItemPrefab;
    public GameObject offerPrefab;
    public GameObject noWorkersNotice;
    public Company company;

    public UIButton hireButton;

    private Worker currentWorker;
    private List<Worker> availableWorkers = new List<Worker>();

    void OnEnable() {
        gridCenter.onFinished = OnCenter;
        company = GameManager.Instance.playerCompany;

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
        if (company.remainingSpace > 0) {
            UIOffer ic = NGUITools.AddChild(gameObject, offerPrefab).GetComponent<UIOffer>();
            ic.bodyText = "Make an offer for " + currentWorker.name + ".";
            ic.offer = 5000;

            UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                ic.Close_();
                if (ic.offer >= currentWorker.minSalary) {
                    // Set the worker's salary to the offer.
                    // Reset the player offer counter.
                    // Hire the worker!
                    currentWorker.salary = ic.offer;
                    currentWorker.recentPlayerOffers = 0;
                    HireWorker_();
                } else {
                    if (++currentWorker.recentPlayerOffers >= 3) {
                        // If you make too many failed offers,
                        // the worker goes off the market for a bit.
                        currentWorker.offMarketTime = 4;

                        UIManager.Instance.Alert("Your offers were too low. I've decided to take a position somewhere else.");

                        RemoveWorker(currentWorker);
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
            UIManager.Instance.Alert("You don't have any space for new workers. Considering laying some people off or expanding to a new location.");
        }
    }
    private void HireWorker_() {
        GameManager.Instance.workerManager.HireWorker(currentWorker);

        RemoveWorker(currentWorker);

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
            hireButton.gameObject.SetActive(false);
            noWorkersNotice.SetActive(true);
        }
    }

    private void OnCenter() {
        // Re-enable the buttons
        // when centering the item after a swipe is complete.
        hireButton.isEnabled = true;
        currentWorker = gridCenter.centeredObject.GetComponent<UIWorker>().worker as Worker;
    }

    private void LoadWorkers() {
        ClearGrid();
        WorkerInsight wi = GameManager.Instance.workerInsight;
        foreach (Worker w in GameManager.Instance.workerManager.AvailableWorkers) {
            availableWorkers.Add(w);
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerItemPrefab);

            switch (wi) {
                case WorkerInsight.Basic:
                    workerItem.GetComponent<UIWorker>().SetBasicWorker(w);
                    break;
                case WorkerInsight.Fuzzy:
                    workerItem.GetComponent<UIWorker>().SetFuzzyWorker(w);
                    break;
                case WorkerInsight.Quant:
                    workerItem.GetComponent<UIWorker>().SetQuantWorker(w);
                    break;
            }
        }
        Adjust();
    }

}
