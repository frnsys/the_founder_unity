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

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
    }

    void Update() {
        string current = string.Format("{0:C0}", company.quarterRevenue);
        string target  = string.Format("{0:C0}", gm.revenueTarget);
        goalLabel.text = string.Format("Quarterly Target: {0}/{1}", current, target);

        if (company.quarterRevenue >= gm.revenueTarget) {
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
