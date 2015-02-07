using UnityEngine;
using System.Collections;

public class UIPerformanceReport : UIAlert {
    public void BuildReport(PerformanceDict results, PerformanceDict deltas, TheBoard board){
        GetLabel("Revenue").text       = string.Format("{0:C0}", results["Quarterly Revenue"]);
        GetLabel("Revenue Delta").text = string.Format("{0}%", deltas["Quarterly Revenue"] * 100);
        GetLabel("The Board").text     = board.BoardStatus();
    }

    private UILabel GetLabel(string labelName) {
        return body.transform.Find("Content/" + labelName).GetComponent<UILabel>();
    }
}
