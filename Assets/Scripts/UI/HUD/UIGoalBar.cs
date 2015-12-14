using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGoalBar : MonoBehaviour {
    private GameManager gm;
    private Company company;

    public UILabel goalLabel;
    public UITexture background;
    public Color defaultColor;
    public Color dangerColor;
    public UIProgressBar progress;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
    }

    void Update() {
        goalLabel.text = string.Format("{0} Profit Target: {1:C0}/{2:C0}", GameManager.Instance.year, company.annualProfit, gm.profitTarget);
        progress.value = company.annualProfit/gm.profitTarget;

        if (company.annualProfit < 0) {
            background.color = dangerColor;
        } else {
            background.color = defaultColor;
        }
    }
}
