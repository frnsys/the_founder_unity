using UnityEngine;
using System.Collections;

public class UIPromo : MonoBehaviour {
    private Promo promo_;
    public Promo promo {
        get { return promo_; }
        set {
            promo_ = value;
            label.text = promo_.name;
            description.text = promo_.description;
            image.mainTexture = promo_.icon;
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture image;

    public void SelectPromo() {
        Company company = GameManager.Instance.playerCompany;
        if (company.developingPromo != null && company.developingPromo != promo_) {
            UIManager.Instance.Confirm("Are you sure want to change the current promotion? You will lose accumulated progress for the current promotion.", delegate() {
                company.StartPromo(promo_);
            }, null);
        } else {
            company.StartPromo(promo_);
        }
    }
}


