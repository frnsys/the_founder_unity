using UnityEngine;
using System.Collections;

public class UILocationItem : MonoBehaviour {
    private Location location_;
    public Location location {
        get { return location_; }
        set {
            location_ = value;
            label.text = location_.name;
            description.text = location_.description;

            if (GameManager.Instance.playerCompany.locations.Contains(location_)) {
                cost.text = "owned";
                expandButton.SetActive(false);
            } else {
                cost.text = "$" + location_.cost.ToString();
            }
        }
    }

    public void ExpandToLocation() {
        UIManager.Instance.Confirm("Are you sure want to expand to " + location_.name + "?", delegate() {
            bool success = GameManager.Instance.playerCompany.ExpandToLocation(location_);

            if (!success) {
                UIManager.Instance.Alert("You don't have enough capital to expand here. Get out.");
            }
        }, null);
    }

    public UILabel label;
    public UILabel description;
    public UILabel cost;
    public GameObject expandButton;
}


