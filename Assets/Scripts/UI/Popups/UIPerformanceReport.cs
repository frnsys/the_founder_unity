using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPerformanceReport : UIAlert {
    public GameObject first;
    public GameObject second;
    public UILabel revenueLabel;
    public UILabel revenueDeltaLabel;
    public UILabel costsLabel;
    public UILabel costsDeltaLabel;
    public UILabel profitLabel;
    public UILabel profitDeltaLabel;
    public UILabel taxesLabel;
    public UILabel taxesTitleLabel;
    public UILabel salariesLabel;
    public UILabel rentLabel;
    public UILabel otherCostsLabel;
    public UILabel boardLabel;
    public UILabel profitTargetLabel;
    public UILabel profitDifferenceLabel;
    public Color posColor;
    public Color negColor;

    public void BuildReport(Company.StatusReport report, TheBoard board) {
        taxesTitleLabel.text = string.Format("Taxes ({0:F0}%)", GameManager.Instance.taxRate * 100);
        taxesLabel.text = string.Format("{0:C0}", report.taxes);
        rentLabel.text = string.Format("{0:C0}", report.rent);
        salariesLabel.text = string.Format("{0:C0}", report.salaries);
        otherCostsLabel.text = string.Format("{0:C0}", report.otherCosts);

        revenueLabel.text = string.Format("{0:C0}", report.revenue);
        revenueDeltaLabel.text = string.Format("{0}%", report.revenueDelta * 100);

        costsLabel.text = string.Format("{0:C0}", report.costs);
        costsDeltaLabel.text = string.Format("{0}%", report.costsDelta * 100);

        profitLabel.text = string.Format("{0:C0}", report.profit);
        profitDeltaLabel.text = string.Format("{0}%", report.profitDelta * 100);

        boardLabel.text = board.BoardStatus();

        profitTargetLabel.text = string.Format("{0:C0}", board.lastProfitTarget);
        profitDifferenceLabel.text = string.Format("{0:C0}", board.lastProfit - board.lastProfitTarget);
    }

    public void ShowNext() {
        first.SetActive(false);
        second.SetActive(true);
    }
}
