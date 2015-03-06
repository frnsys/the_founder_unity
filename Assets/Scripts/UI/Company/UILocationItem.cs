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
            grid.gameObject.SetActive(true);
        } else {
            cost.text = string.Format("{0:C0}", location_.cost);
            expandButton.SetActive(true);
            grid.gameObject.SetActive(false);
        }
        ShowInfrastructure();

        RenderEffects(location_.effects);
        AdjustEffectsHeight();
    }

    void Awake() {
        playerCompany = GameManager.Instance.playerCompany;
    }

    public void ExpandToLocation() {
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

    public UILabel cost;
    public UILabel label;
    public UILabel description;
    public UILabel market;
    public GameObject expandButton;

    public List<UIInfrastructureItem> infItems;
    public UIGrid grid;
    private Company playerCompany;
    private void ShowInfrastructure() {
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            if (location.capacity[t] == 0) {
                infItem.gameObject.SetActive(false);
            } else {
                infItem.amountLabel.text = string.Format("{0}", location.capacity[t]);
            }
        }
        grid.Reposition();
    }

    void Update() {
        UpdateEffectWidths();
    }
}


