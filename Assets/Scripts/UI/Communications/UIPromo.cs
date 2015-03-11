using UnityEngine;
using System.Collections;

public class UIPromo : MonoBehaviour {
    private Promo promo_;
    public Promo promo {
        get { return promo_; }
        set {
            promo_ = value;
            label.text = promo_.name;
            image.mainTexture = promo_.icon;
            cost.text = string.Format("{0:C0}", promo_.cost);
            description.text = promo_.description;
        }
    }

    public UILabel label;
    public UILabel cost;
    public UILabel description;
    public UITexture image;

    public void SelectPromo() {
        if (!GameManager.Instance.playerCompany.StartPromo(promo_)) {
            UIManager.Instance.Alert("You don't have the cash for this campaign.");
        }
    }
}


