using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGoalBar : MonoBehaviour {
    private GameManager gm;
    private Company company;

    public UILabel goalLabel;
    public UITexture background;
    public Color successColor;
    public Color defaultColor;
    public Color dangerColor;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
    }

    void Update() {
        goalLabel.text = string.Format("{0} Profit Target: {1:C0}/{2:C0}", GameManager.Instance.year, company.annualProfit, gm.profitTarget);

        if (company.annualProfit < 0) {
            background.color = dangerColor;
        } else if (company.annualProfit >= gm.profitTarget) {
            background.color = successColor;
        } else {
            background.color = defaultColor;
        }
    }

    IEnumerator AnimateToColor(Color c) {
        Color start = background.color;
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            // SmoothStep gives us a bit of easing.
            background.color = Color.Lerp(start, c, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
