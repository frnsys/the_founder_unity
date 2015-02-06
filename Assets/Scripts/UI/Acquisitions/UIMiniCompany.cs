using UnityEngine;
using System.Collections;

public class UIMiniCompany : UIEffectItem {
    private MiniCompany _company;
    public MiniCompany company {
        get { return _company; }
        set {
            _company = value;
            nameLabel.text = _company.name;
            descLabel.text = _company.description;
            costLabel.text = string.Format("{0:C0}", _company.cost);
            revenueLabel.text = string.Format("Generates ~{0:C0} per month.", _company.revenue);

            RenderEffects(_company.effects);
            AdjustEffectsHeight();

            // Check if the company is already owned.
            if (playerCompany.companies.Contains(_company)) {
                button.isEnabled = false;
                buttonLabel.text = "Owned";
            }
        }
    }

    private Company playerCompany;

    public UILabel nameLabel;
    public UILabel descLabel;
    public UILabel costLabel;
    public UILabel revenueLabel;
    public UILabel buttonLabel;
    public UITexture logo;
    public UIButton button;

    void Awake() {
        playerCompany = GameManager.Instance.playerCompany;
    }

    public void DecideBuy(GameObject obj) {
        UIManager.Instance.Confirm("Acquiring " + _company.name + " will cost you " + costLabel.text, delegate() {
                if (playerCompany.BuyCompany(_company)) {
                    company = _company;
                } else {
                    UIManager.Instance.Alert("You can't afford to acquire this company.");
                }
        }, null);
    }
}
