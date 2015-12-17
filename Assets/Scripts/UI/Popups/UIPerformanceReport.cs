using UnityEngine;
using System.Collections;

public class UIPerformanceReport : UIAlert {
    public GameObject first;
    public GameObject second;
    public UILabel revenueLabel;
    public UILabel revenueDeltaLabel;
    public UILabel costsLabel;
    public UILabel costsDeltaLabel;
    public UILabel profitLabel;
    public UILabel profitDeltaLabel;
    public UILabel boardLabel;

    public void BuildReport(Dictionary<string, float> results, TheBoard board){
        revenueLabel.text = string.Format("{0:C0}", results["Annual Revenue"]);
        revenueDeltaLabel.text = string.Format("{0}%", deltas["Annual Revenue Delta"] * 100);

        costsLabel.text = string.Format("{0:C0}", results["Annual Costs"]);
        costsDeltaLabel.text = string.Format("{0}%", deltas["Annual Costs Delta"] * 100);

        profitLabel.text = string.Format("{0:C0}", results["Annual Profit"]);
        profitDeltaLabel.text = string.Format("{0}%", deltas["Annual Profit Delta"] * 100);

        boardLabel.text = board.BoardStatus();
    }

    public void ShowNext() {
        first.SetActive(false);
        second.SetActive(true);
    }
}
