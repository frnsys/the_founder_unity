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
        Company company = GameManager.Instance.playerCompany;
        if (company.developingRecruitment != null && company.developingRecruitment != recruitment_) {
            UIManager.Instance.Confirm("Are you sure want to change the current recruitment strategy? You will lose accumulated progress for the current one.", delegate() {
                company.StartRecruitment(recruitment_);
            }, null);
        } else {
            company.StartRecruitment(recruitment_);
        }
    }
}


