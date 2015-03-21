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
        }
    }

    public UILabel label;
    public UILabel cost;
    public UITexture image;
    public UIGrid starsGrid;

    void OnClick() {
        UIManager.Instance.Confirm(string.Format("Are you sure want to recruit this way? It will cost you {0:C0}.", recruitment_.cost), delegate() {
            if (!GameManager.Instance.playerCompany.StartRecruitment(recruitment_)) {
                UIManager.Instance.Alert("You don't have the cash to recruit this way.");
            }
        }, null);
    }

    public int stars {
        set {
            int v = value + 1;
            for (int i=0; i < v; i++) {
                starsGrid.transform.GetChild(i).gameObject.SetActive(true);
            }
            starsGrid.Reposition();
        }
    }
}


