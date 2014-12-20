using UnityEngine;
using System.Collections;

public class UITechnologyItem : MonoBehaviour {
    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            label.text = technology_.name;
            description.text = technology_.description;

            // TO DO Show effects?
        }
    }

    public UILabel label;
    public UILabel description;
}


