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
        // TO DO
    }

    public UILabel label;
    public UILabel description;
    public UILabel cost;
    public GameObject expandButton;
}


