using UnityEngine;
using System.Collections;

// A popup to make an offer to a prospective employee.
public class UIOffer : UIConfirm {
    public UILabel offerLabel;
    public UILabel monthlyCostLabel;
    public UILabel hiringFeeLabel;
    public GameObject humanUI;

    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = string.Format("{0:C0}", offer_);
            monthlyCostLabel.text = string.Format("({0:C0}/mo)", offer_/12);
            hiringFeeLabel.text = string.Format("{0:C0} hiring fee", offer_ * 0.1);
        }
    }

    protected override void Extend(int amount) {
        amount = (amount/2) + 130;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }

    public void Increment() {
        offer += 5000;
    }

    public void Decrement() {
        offer -= 5000;
        if (offer < 0)
            offer = 0;
    }

    public void SetRobotWorker(string text, float cost) {
        offerLabel.text = string.Format("{0:C0}", cost);
        bodyText = text;
        humanUI.SetActive(false);
    }
}
