using UnityEngine;
using System.Collections;

public class UIVerticalItem : MonoBehaviour {
    private Vertical vertical_;
    public Vertical vertical {
        get { return vertical_; }
        set {
            vertical_ = value;
            label.text = vertical_.name;
            description.text = vertical_.description;
            if (GameManager.Instance.playerCompany.verticals.Contains(vertical_)) {
                cost.text = "operating in this vertical";
                expandButton.SetActive(false);
            } else {
                cost.text = "$" + vertical_.cost.ToString();
            }
        }
    }

    public void ExpandToVertical() {
        UIManager.Instance.Confirm("Are you sure want to expand into " + vertical_.name + "?", delegate() {
            bool success = GameManager.Instance.playerCompany.ExpandToVertical(vertical_);

            if (!success) {
                UIManager.Instance.Alert("You don't have enough capital to break into this industry. Get out of my office.");
            }
        }, null);
    }

    public UILabel label;
    public UILabel description;
    public UILabel cost;
    public GameObject expandButton;
}


