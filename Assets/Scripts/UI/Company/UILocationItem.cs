using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILocationItem : UIEffectItem {
    private Location location_;
    public Location location {
        get { return location_; }
        set {
            location_ = value;
            DisplayLocation();
        }
    }

    private void DisplayLocation() {
        label.text = location_.name;
        description.text = location_.description;
        market.text = string.Format("Increases {0} market share.", MarketManager.NameForMarket(location_.market));

        if (playerCompany.HasLocation(location_)) {
            cost.text = "owned";
            expandButton.SetActive(false);
        } else {
            cost.text = string.Format("{0:C0}", location_.cost);
            expandButton.SetActive(true);
        }

        RenderEffects(location_.effects);
        AdjustEffectsHeight();
    }

    void Awake() {
        playerCompany = GameManager.Instance.playerCompany;
    }

    public void ExpandToLocation() {
        if (!locked) {
            UIManager.Instance.Confirm("Are you sure want to expand to " + location_.name + "?", delegate() {
                bool success = playerCompany.ExpandToLocation(location_);

                if (!success) {
                    UIManager.Instance.Alert("You don't have enough capital to expand here. Get out.");
                } else {
                    // Re-display the location as an owned one.
                    DisplayLocation();
                }
            }, null);
        }
    }

    public UILabel cost;
    public UILabel label;
    public UILabel description;
    public UILabel market;
    public GameObject expandButton;

    private Company playerCompany;

    void Update() {
        UpdateEffectWidths();
    }

    public bool locked;
    public void Lock() {
        GetComponent<UIWidget>().alpha = 0.3f;
        transform.Find("Lock").gameObject.SetActive(true);
        label.text = "Locked";
        description.gameObject.SetActive(false);
        market.gameObject.SetActive(false);
        effectGrid.gameObject.SetActive(false);
        locked = true;
    }
}


