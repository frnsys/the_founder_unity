using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UINegotiation : UIWindow {
    public UILabel offerLabel;
    public UILabel probabilitySuccessLabel;
    public UILabel nameLabel;
    public UILabel personalInfoLabel;
    public UILabel turnsLabel;
    public MeshRenderer employee;

    private AWorker worker;
    private UIHireWorkers hireWorkersWindow;
    private float probAccept;

    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = string.Format("{0:C0}/mo", offer_);

            probAccept = AcceptanceProb(offer_);
            string color = "7BD6A4";
            if (probAccept <= 0.55) {
                color = "FF1E1E";
            } else if (probAccept <= 0.75) {
                color = "F1B71A";
            }
            probabilitySuccessLabel.text = string.Format("[c][{0}]{1:F0}% success[-][/c]", color, probAccept * 100);
        }
    }

    public void Setup(AWorker w, UIHireWorkers hww) {
        nameLabel.text = w.name;
        if (w.material != null)
            employee.material = w.material;

        if (GameManager.Instance.workerInsight) {
            personalInfoLabel.text = string.Format("- {0}", string.Join("\n- ", w.personalInfos.ToArray()));
        } else {
            personalInfoLabel.text = "";
        }

        worker = w;
        hireWorkersWindow = hww;
        offer = 40000;
    }

    public void Increment() {
        offer += 2000;
    }

    public void Decrement() {
        offer -= 2000;
        if (offer < 0)
            offer = 0;
    }

    public void MakeOffer() {
        UIManager.Instance.Confirm(string.Format("There will be a {0:C0} hiring fee (0.1%). Is that ok?", offer_ * 0.1f), delegate {
            if (Random.value <= probAccept) {
                Debug.Log("offer successful!");
            } else {
                TakeTurn();
            }
        } , null);
    }

    private float AcceptanceProb(int off) {
        float minSal = worker.MinSalaryForCompany(GameManager.Instance.playerCompany);
        float diff = off - minSal;
        if (diff >= 0) {
            return 0.99f;
        } else {
            float x = -diff/minSal;
            return Mathf.Max(0, 0.99f - (((1/(-x-1)) + 1)*2));
        }
    }

    public void ChooseDialogue() {
        Debug.Log("chose dialogue");
        TakeTurn();
    }

    private void TakeTurn() {
        worker.turnsTaken +=1;
        turnsLabel.text = string.Format("{0}/5 turns", worker.turnsTaken + 1);
        if (worker.turnsTaken >= 5) {
            // After all turns,
            // the worker goes off the market for a bit.
            worker.offMarketTime = 4;
            UIManager.Instance.Alert("Your offers were too low. I've decided to take a position somewhere else.");
            hireWorkersWindow.RemoveWorker(worker);
            base.Close();
        }
    }

    public void Close() {
        if (worker.turnsTaken > 0) {
            UIManager.Instance.Confirm("Are you sure want to leave negotiations? Your turns will take some time to reset.", delegate {
                base.Close();
            } , null);
        } else {
            base.Close();
        }
    }
}