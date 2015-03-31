using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Displays cash and time info.
public class UIStatusBar : MonoBehaviour {
    private GameManager gm;

    public UILabel cashLabel;
    public UILabel hypeLabel;
    public UILabel researchLabel;
    public UILabel yearLabel;
    public UILabel monthLabel;
    public UIGrid weekGrid;

    public Color activeWeekColor;
    public Color defaultWeekColor;

    private int week;

    void OnEnable() {
        gm = GameManager.Instance;
        week = gm.week;
        SetWeek();
    }

    public Color posCashColor;
    public Color posCashShadow;
    public Color negCashColor;
    public Color negCashShadow;
    private float cash;
    void Update() {
        cash = gm.playerCompany.cash.value;
        if (cash <= 0) {
            cashLabel.color = negCashColor;
            cashLabel.effectColor = negCashShadow;
        } else {
            cashLabel.color = posCashColor;
            cashLabel.effectColor = posCashShadow;
        }
        cashLabel.text = string.Format("{0:C0}", cash);
        hypeLabel.text = string.Format("{0:0.#}x hype", gm.playerCompany.publicity.value);
        researchLabel.text = string.Format("{0} research", gm.playerCompany.researchPoints);
        yearLabel.text = gm.year.ToString();
        monthLabel.text = gm.month;

        // If the week has changed,
        // update the UI.
        if (week != gm.week) {
            week = gm.week;
            SetWeek();
        }
    }

    private void SetWeek() {
        List<Transform> gridChildren = weekGrid.GetChildList();
        for (int i=0; i<gridChildren.Count; i++) {
            if (i == gm.week) {
                gridChildren[i].GetComponent<UITexture>().color = activeWeekColor;
            } else {
                gridChildren[i].GetComponent<UITexture>().color = defaultWeekColor;
            }
        }
    }
}
