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

            // TO DO Show effects?
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture image;

    public void SelectTech() {
        ResearchManager rm = GameManager.Instance.researchManager;
        if (rm.technology != null && rm.technology != technology_) {
            UIManager.Instance.Confirm("Are you sure want to change what you're researching? You will lose accumulated progress for the current technology.", delegate() {
                rm.BeginResearch(technology_);
            }, null);
        } else {
            rm.BeginResearch(technology_);
        }
    }
}


