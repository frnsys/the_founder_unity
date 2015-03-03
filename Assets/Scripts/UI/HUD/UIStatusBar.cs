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

    // Economy icon.
    public Color depressionColor;
    public Color recessionColor;
    public Color neutralColor;
    public Color expansionColor;
    public UITexture economyIcon;

    private int week;

    void OnEnable() {
        gm = GameManager.Instance;
        week = gm.week;
        SetWeek();
    }

    void Update() {
        cashLabel.text = string.Format("{0:C0}", gm.playerCompany.cash.value);
        hypeLabel.text = string.Format("{0:F0} hype", gm.playerCompany.publicity.value);
        researchLabel.text = string.Format("{0:0} research", gm.playerCompany.researchPoints);
        yearLabel.text = gm.year.ToString();
        monthLabel.text = gm.month.ToUpper();

        switch (gm.economy) {
            case Economy.Depression:
                economyIcon.color = depressionColor;
                break;
            case Economy.Recession:
                economyIcon.color = recessionColor;
                break;
            case Economy.Neutral:
                economyIcon.color = neutralColor;
                break;
            case Economy.Expansion:
                economyIcon.color = expansionColor;
                break;
        }

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
