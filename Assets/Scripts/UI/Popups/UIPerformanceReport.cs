using UnityEngine;
using System.Collections;

public class UIPerformanceReport : UIAlert {
    public GameObject first;
    public GameObject second;
    public UILabel revenueLabel;
    public UILabel revenueDeltaLabel;
    public UILabel boardLabel;

    public void BuildReport(PerformanceDict results, PerformanceDict deltas, TheBoard board){
        revenueLabel.text = string.Format("{0:C0}", results["Quarterly Revenue"]);
        revenueDeltaLabel.text = string.Format("{0}%", deltas["Quarterly Revenue"] * 100);
        boardLabel.text = board.BoardStatus();
    }

    public void ShowNext() {
        first.SetActive(false);
        second.SetActive(true);
    }
}
