using UnityEngine;
using System.Collections;

public class UIRecruitment : MonoBehaviour {
    private Recruitment recruitment_;
    public Recruitment recruitment {
        get { return recruitment_; }
        set {
            recruitment_ = value;
            label.text = recruitment_.name;
            image.mainTexture = recruitment_.icon;
            cost.text = string.Format("{0:C0}", recruitment_.cost);
            description.text = recruitment_.description;
        }
    }

    public UILabel label;
    public UILabel cost;
    public UILabel description;
    public UITexture image;

    public void SelectRecruitment() {
        GameManager.Instance.playerCompany.StartRecruitment(recruitment_);
    }
}


