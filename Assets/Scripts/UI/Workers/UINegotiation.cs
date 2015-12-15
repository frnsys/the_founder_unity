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

    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = string.Format("{0:C0}/mo", offer_);

            float successProb = 50f; // TODO
            probabilitySuccessLabel.text = string.Format("{0}% success", successProb);
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
    }

    public void Increment() {
        offer += 5000;
    }

    public void Decrement() {
        offer -= 5000;
        if (offer < 0)
            offer = 0;
    }

    public void MakeOffer() {
        //TODO roll to see if offer is accepted
        UIManager.Instance.Confirm(string.Format("There will be a {0:C0} hiring fee (0.1%).", 1000), delegate {
            Debug.Log("Offer made");
            TakeTurn();
        } , null);
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