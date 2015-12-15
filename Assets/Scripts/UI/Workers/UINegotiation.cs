using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UINegotiation : UIWindow {
    public UILabel offerLabel;
    public UILabel hiringFeeLabel;
    public UILabel probabilitySuccessLabel;
    public UILabel nameLabel;
    public UILabel personalInfoLabel;
    public UILabel turnsLabel;
    public MeshRenderer employee;

    private Worker worker;
    private int turns = 0;

    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = string.Format("{0:C0}/mo", offer_);
            hiringFeeLabel.text = string.Format("{0:C0} hiring fee", offer_ * 0.1);

            float successProb = 50f; // TODO
            probabilitySuccessLabel.text = string.Format("{0}% success", successProb);
        }
    }

    public void Setup(Worker w) {
        nameLabel.text = w.name;
        if (w.material != null)
            employee.material = w.material;

        if (GameManager.Instance.workerInsight) {
            personalInfoLabel.text = string.Format("- {0}", string.Join("\n- ", w.personalInfos.ToArray()));
        }

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
        } , null);
    }

    public void Close() {
        if (turns > 0) {
            UIManager.Instance.Confirm("Are you sure want to leave negotiations? Your turns will take some time to reset.", delegate {
                base.Close();
            } , null);
        } else {
            base.Close();
        }
    }
}