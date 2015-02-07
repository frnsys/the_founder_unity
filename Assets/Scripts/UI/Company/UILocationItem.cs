using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILocationItem : MonoBehaviour {
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

        if (playerCompany.HasLocation(location_)) {
            location_ = Location.Find(location_, playerCompany.locations);
            cost.text = "owned";
            expandButton.SetActive(false);
            grid.gameObject.SetActive(true);
            capacity.text = "";
            ShowInfrastructure();
        } else {
            cost.text = string.Format("{0:C0}", location_.cost);
            expandButton.SetActive(true);
            grid.gameObject.SetActive(false);
            capacity.text = "Adds capacity for:\n" + location_.capacity.ToString();
        }
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
    public UILabel capacity;
    public UILabel description;
    public GameObject expandButton;

    public GameObject infrastructureItemPrefab;
    public List<UIInfrastructureItem> infItems;
    public UIGrid grid;
    private Company playerCompany;
    private void ShowInfrastructure() {
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            if (location.capacity[t] == 0) {
                infItem.gameObject.SetActive(false);
            } else {
                infItem.costLabel.text = string.Format("{0:C0}/mo each", location.capacity.baseCosts[t]);

                // Setup button actions.
                Infrastructure.Type lT = t;
                UIEventListener.VoidDelegate buyInf = delegate(GameObject obj) {
                    Infrastructure inf = Infrastructure.ForType(lT);
                    if (!playerCompany.BuyInfrastructure(inf, location)) {
                        UIManager.Instance.Alert("You don't have enough capital for a new piece of infrastructure.");
                    };
                    UpdateButtons();
                };
                UIEventListener.VoidDelegate desInf = delegate(GameObject obj) {
                    Infrastructure inf = Infrastructure.ForType(lT);
                    playerCompany.DestroyInfrastructure(inf, location);
                    UpdateButtons();
                };

                // Bind actions to buttons.
                UIEventListener.Get(infItem.buyButton.gameObject).onClick += buyInf;
                UIEventListener.Get(infItem.desButton.gameObject).onClick += desInf;
            }
        }

        UpdateButtons();
        grid.Reposition();
    }

    void Update() {
        Infrastructure capacity = location.capacity;
        Infrastructure infra    = location.infrastructure;

        // Update the amount of infrastructure against the total capacity.
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            infItem.usedAndCapacityLabel.text = string.Format("{0}/{1}", infra[t], capacity[t]);
        }
    }

    private void UpdateButtons() {
        // Update the buy/destroy buttons for infrastructure (disable/enable them).
        Infrastructure infra    = location.infrastructure;
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            Infrastructure inf = Infrastructure.ForType(t);
            infItem.buyButton.isEnabled = playerCompany.HasCapacityFor(inf);
            infItem.desButton.isEnabled = !(infra[t] == 0);
        }
    }
}


