using UnityEngine;
using System.Collections;

public class UIOnboardingCofounder : MonoBehaviour {
    private Founder cofounder_;
    public Founder cofounder {
        get { return cofounder_; }
        set {
            cofounder_ = value;
            label.text = cofounder_.name;
            description.text = cofounder_.description;
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture background;
}


