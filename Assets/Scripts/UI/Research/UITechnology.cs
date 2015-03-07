using UnityEngine;
using System.Collections;

public class UITechnology : MonoBehaviour {
    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            label.text = technology_.name;
            description.text = technology_.description;
            image.mainTexture = technology_.icon;
            cost.text = string.Format("Costs {0} research", technology_.cost);

            // TO DO Show effects?
        }
    }

    public UILabel label;
    public UILabel cost;
    public UILabel description;
    public UITexture image;

    public void SelectTech() {
        UIManager.Instance.Confirm("Are you sure you want to research this technology?", delegate() {
            if (!GameManager.Instance.playerCompany.BuyTechnology(technology_)) {
                UIManager.Instance.Alert("You don't have enough research for this technology.");
            }
        }, null);
    }
}


