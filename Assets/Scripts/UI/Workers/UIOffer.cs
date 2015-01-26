using UnityEngine;
using System.Collections;

// A popup to make an offer to a prospective employee.
public class UIOffer : UIConfirm {
    public UILabel offerLabel;
    public UILabel monthlyCostLabel;
    public UILabel hiringFeeLabel;

    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = "$" + offer_.ToString();
            monthlyCostLabel.text = "($" + (offer_/12).ToString() + "/mo)";
            hiringFeeLabel.text = "$" + (offer_ * 0.1).ToString() + " hiring fee";
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
}
