using UnityEngine;
using System.Collections;

public class UIPromo : MonoBehaviour {
    private Promo promo_;
    public Promo promo {
        get { return promo_; }
        set {
            promo_ = value;
            label.text = promo_.name;
            image.mainTexture = promo_.icon;
            cost.text = string.Format("{0:C0}", promo_.cost);
        }
    }

    public UILabel label;
    public UILabel cost;
    public UITexture image;
    public UIGrid starsGrid;

    void OnClick() {
        if (!locked) {
            UIManager.Instance.Confirm(string.Format("Are you sure want to run this campaign? It will cost you {0:C0}.", promo_.cost), delegate() {
                if (!GameManager.Instance.playerCompany.StartPromo(promo_)) {
                    UIManager.Instance.Alert("You don't have the cash for this campaign.");
                }
            }, null);
        }
    }

    public int stars {
        set {
            int v = value + 1;

            // Odd values have half stars.
            if (v % 2 == 1) {
                starsGrid.transform.GetChild(starsGrid.transform.childCount - 1).gameObject.SetActive(true);
            }
            for (int i=0; i < v/2; i++) {
                starsGrid.transform.GetChild(i).gameObject.SetActive(true);
            }
            starsGrid.Reposition();
        }
    }

    private bool locked;
    public void Lock() {
        GetComponent<UIWidget>().alpha = 0.3f;
        transform.Find("Lock").gameObject.SetActive(true);
        locked = true;
    }
}


