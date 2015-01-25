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
            cost.text = "$" + location_.cost.ToString();
            expandButton.SetActive(true);
            grid.gameObject.SetActive(false);
            capacity.text = "Adds capacity for:\n" + location_.capacity.ToString();
        }
    }

    void Awake() {
        gridItems = new List<GameObject>();
        types = new List<Infrastructure.Type>();
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
    public UIGrid grid;
    private Company playerCompany;
    private List<GameObject> gridItems;
    private List<Infrastructure.Type> types;
    private void ShowInfrastructure() {
        // Create an item for each infrastructure type supported by the location.
        foreach(KeyValuePair<Infrastructure.Type, int> i in location.capacity) {
            if (i.Value > 0) {
                Infrastructure.Type t = i.Key;
                GameObject item = NGUITools.AddChild(grid.gameObject, infrastructureItemPrefab);
                item.transform.Find("Label").GetComponent<UILabel>().text = t.ToString();
                item.transform.Find("Cost").GetComponent<UILabel>().text = "$" + location.capacity.baseCosts[t].ToString() + "/mo each";

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
                GameObject buyButton = item.transform.Find("Buy Button").gameObject;
                UIEventListener.Get(buyButton).onClick += buyInf;

                GameObject desButton = item.transform.Find("Destroy Button").gameObject;
                UIEventListener.Get(desButton).onClick += desInf;

                gridItems.Add(item);
                types.Add(t);
            }
        }
        UpdateButtons();
        grid.Reposition();
    }

    void Update() {
        Infrastructure capacity = location.capacity;
        Infrastructure infra    = location.infrastructure;

        // Update the amount of infrastructure against the total capacity.
        for (int i=0; i<gridItems.Count; i++) {
            GameObject item = gridItems[i];
            Infrastructure.Type t = types[i];
            item.transform.Find("Used and Capacity").GetComponent<UILabel>().text = infra[t].ToString() + "/" +  capacity[t].ToString();
        }
    }

    private void UpdateButtons() {
        Infrastructure capacity = location.capacity;
        Infrastructure infra    = location.infrastructure;

        // Update the buy/destroy buttons for infrastructure (disable/enable them).
        for (int i=0; i<gridItems.Count; i++) {
            GameObject item = gridItems[i];
            Infrastructure.Type t = types[i];

            GameObject buyButton = item.transform.Find("Buy Button").gameObject;
            GameObject desButton = item.transform.Find("Destroy Button").gameObject;

            Infrastructure inf = Infrastructure.ForType(t);
            buyButton.GetComponent<UIButton>().isEnabled = playerCompany.HasCapacityFor(inf);
            desButton.GetComponent<UIButton>().isEnabled = !(infra[t] == 0);
        }
    }
}


