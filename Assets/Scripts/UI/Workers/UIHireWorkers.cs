using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHireWorkers : UIFullScreenPager {
    private Company company;
    private GameManager gm;

    public GameObject workerItemPrefab;
    public GameObject noWorkersNotice;
    public GameObject negotiationPrefab;
    public GameObject offerPrefab;

    private List<AWorker> availableWorkers = new List<AWorker>();

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;

        // Pause the hiring window,
        // so AI companies aren't hiring candidates you're reviewing.
        if (GameManager.hasInstance)
            GameManager.Instance.Pause();
    }

    void OnDisable() {
        if (GameManager.hasInstance)
            GameManager.Instance.Resume();
    }

    public void HireRobotWorker(AWorker worker) {
        if (company.remainingSpace > 0) {
            UIOffer ic = NGUITools.AddChild(gameObject, offerPrefab).GetComponent<UIOffer>();
            ic.SetRobotWorker(string.Format("The {0} model costs", worker.name), worker.baseMinSalary);

            UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                ic.Close_();
                GameManager.Instance.workerManager.HireWorker(worker);
                RemoveWorker(worker);
                UIManager.Instance.Alert("BOOT SEQUENCE COMPLETE. READY TO SERVE " + company.name + ".");
            };

            UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
                ic.Close_();
            };

            UIEventListener.Get(ic.yesButton).onClick += yesAction;
            UIEventListener.Get(ic.noButton).onClick += noAction;

        } else {
            UIManager.Instance.Alert("You don't have any space for new workers. Consider laying some people off or upgrading your office.");
        }
    }

    public void HireWorker(AWorker worker) {
        if (company.remainingSpace > 0) {
            // hacky, b/c UIManager doesn't allow multiple simultaneous popups
            GameObject negotiationPopup = NGUITools.AddChild(UIManager.Instance.windowsPanel, negotiationPrefab);
            negotiationPopup.GetComponent<UIWidget>().SetAnchor(UIManager.Instance.windowsPanel.gameObject, 0, 0, 0, 0);

            //UIOffer ic = NGUITools.AddChild(gameObject, offerPrefab).GetComponent<UIOffer>();

            //ic.bodyText = "Make an offer for " + worker.name + ".";
            //ic.offer = 40000;

            //UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                //ic.Close_();
                //float minSal = worker.MinSalaryForCompany(company);
                //if (ic.offer >= minSal) {
                    //// Set the worker's salary to the offer.
                    //// Reset the player offer counter.
                    //// Hire the worker!
                    //worker.salary = ic.offer;
                    //worker.recentPlayerOffers = 0;
                    //HireWorker_(worker);
                //} else {
                    //if (++worker.recentPlayerOffers >= 3) {
                        //// If you make too many failed offers,
                        //// the worker goes off the market for a bit.
                        //worker.offMarketTime = 4;

                        //UIManager.Instance.Alert("Your offers were too low. I've decided to take a position somewhere else.");

                        //RemoveWorker(worker);
                    //} else {
                        //if (minSal - ic.offer >= 50000) {
                            //UIManager.Instance.Alert("That is insultingly low. I'm worth way more than that.");
                        //} else if (minSal - ic.offer >= 30000) {
                            //UIManager.Instance.Alert("I made more than that at my last job. How about more?");
                        //} else if (minSal - ic.offer >= 10000) {
                            //UIManager.Instance.Alert("That's a little low. I think I deserve more.");
                        //} else {
                            //UIManager.Instance.Alert("This is starting to look acceptable, but I need to see a bit more.");
                        //}
                    //}
                //}
            //};

            //UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
                //ic.Close_();
            //};


            //UIEventListener.VoidDelegate warning = delegate(GameObject obj) {
                //UIManager.Instance.Confirm("This is your final offer. Are you sure?", delegate() {
                    //yesAction(null);
                //}, null);
            //};

            //if (worker.recentPlayerOffers == 2) {
                //UIEventListener.Get(ic.yesButton).onClick += warning;
            //} else {
                //UIEventListener.Get(ic.yesButton).onClick += yesAction;
            //}

            //UIEventListener.Get(ic.noButton).onClick += noAction;

        } else {
            UIManager.Instance.Alert("You don't have any space for new workers. Consider laying some people off or expanding to a new location.");
        }
    }
    private void HireWorker_(AWorker worker) {
        GameManager.Instance.workerManager.HireWorker(worker);
        RemoveWorker(worker);

        // TO DO make this fancier
        UIManager.Instance.Alert("It's been my lifelong dream to work for " + company.name + "!!");
    }

    private void RemoveWorker(AWorker worker) {
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

    public void LoadWorkers(List<AWorker> workers) {
        ClearGrid();
        availableWorkers.Clear();
        bool wq = GameManager.Instance.workerQuant;
        foreach (AWorker w in workers) {
            availableWorkers.Add(w);
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, workerItemPrefab);

            UIWorker uiw = workerItem.GetComponent<UIWorker>();
            AWorker worker = w;
            UIEventListener.Get(uiw.button.gameObject).onClick += delegate(GameObject obj) {
                if (worker.robot) {
                    HireRobotWorker(worker);
                } else {
                    HireWorker(worker);
                }
            };

            if (w.robot) {
                uiw.worker = w;
            } else if (!wq) {
                uiw.SetBasicWorker(w);
            } else {
                uiw.SetQuantWorker(w);
            }
        }
        Adjust();
    }
}
