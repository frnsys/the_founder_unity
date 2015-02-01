using UnityEngine;
using System.Collections;

public class UIPerformanceReport : UIAlert {
    public void BuildReport(PerformanceDict results, PerformanceDict deltas, TheBoard board){

        GetLabel("Revenue").text       = "$" + results["Annual Revenue"].ToString();
        GetLabel("Revenue Delta").text = (deltas["Annual Revenue"] * 100).ToString() + "%";
        GetLabel("The Board").text     = board.BoardStatus();
    }

    private UILabel GetLabel(string labelName) {
        return body.transform.Find("Content/" + labelName).GetComponent<UILabel>();
    }
}
