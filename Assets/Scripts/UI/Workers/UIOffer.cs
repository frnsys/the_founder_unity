using UnityEngine;
using System.Collections;

// A popup to make an offer to a prospective employee.
public class UIOffer : UIConfirm {
    public UILabel offerLabel;
    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = "$" + offer_.ToString();
        }
    }

    protected override void Extend(int amount) {
        amount = (amount/2) + 100;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }

    public void Increment() {
        offer += 5000;
    }

    public void Decrement() {
        offer -= 5000;
    }
}
