using UnityEngine;
using System.Collections;

public class UITechnology : UIEffectItem {
    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            label.text = technology_.name;
            description.text = technology_.description;
            image.mainTexture = technology_.icon;
            cost.text = string.Format("Costs {0} research", technology_.cost);

            RenderEffects(technology_.effects);
            AdjustEffectsHeight();
        }
    }

    public bool researched {
        set {
            button.isEnabled = !value;
            if (value) {
                buttonLabel.text = "Researched";
            } else {
                buttonLabel.text = "Research";
            }
        }
    }

    public UILabel label;
    public UILabel cost;
    public UILabel description;
    public UILabel buttonLabel;
    public UIButton button;
    public UITexture image;

    public void SelectTech() {
        UIManager.Instance.Confirm("Are you sure you want to research this technology?", delegate() {
            if (!GameManager.Instance.playerCompany.BuyTechnology(technology_)) {
                UIManager.Instance.Alert("You don't have enough research for this technology.");
            } else {
                researched = true;
            }
        }, null);
    }

    void Update() {
        UpdateEffectWidths();
    }
}


