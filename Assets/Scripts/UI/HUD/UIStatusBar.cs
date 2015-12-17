using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Displays cash and time info.
public class UIStatusBar : MonoBehaviour {
    private GameManager gm;

    public UILabel cashLabel;
    public UILabel yearLabel;

    void OnEnable() {
        gm = GameManager.Instance;
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
        yearLabel.text = gm.year.ToString();
    }
}
