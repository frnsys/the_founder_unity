using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIAccounting : MonoBehaviour {
    private GameManager gm;
    private Company company;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;

        float monthlySalaries = 0;
        foreach (Worker worker in company.workers) {
            monthlySalaries += worker.monthlyPay;
        }
        salaries.text = string.Format("${0:n}", monthlySalaries) + " per month in salaries";

        float monthlyRent = 0;
        float monthlyInf = 0;
        foreach (Location loc in company.locations) {
            monthlyRent += loc.cost;

            // We have to add up location infrastructure cost in this way,
            // so we incorporate the cost of the infrastructure for the location.
            monthlyInf += loc.infrastructure.cost;
        }
        rent.text = string.Format("${0:n}", monthlyRent) + " per month in rent";
        inf.text = string.Format("${0:n}", monthlyInf) + " per month in infrastructure costs";

        numWorkerLocations.text = company.workers.Count.ToString() + " employees across " + company.locations.Count.ToString() + " locations";

        PerformanceDict lastQuarter = company.lastQuarterPerformance;
        if (lastQuarter == null) {
            pastRevenue.text = "Revenue: $0";
            pastProfit.text = "Profit: $0";
        } else {
            float revs = lastQuarter["Quarterly Revenue"];
            float cost = lastQuarter["Quarterly Costs"];
            pastRevenue.text = "Revenue: " + string.Format("${0:n}", revs);
            pastProfit.text = "Profit: " + string.Format("${0:n}", revs-cost);
        }
    }

    void Update() {
        PerformanceDict snapshot = company.quarterSnapshot;
        float revs = snapshot["Revenue"];
        float cost = snapshot["Costs"];
        currentRevenue.text = "Revenue: " + string.Format("${0:n}", revs);
        currentProfit.text = "Profit: " + string.Format("${0:n}", revs-cost);

        researchBudget.text = company.researchInvestment.ToString();
    }

    public void IncreaseResearchBudget() {
        company.researchInvestment += 10000;
    }

    public void DecreaseResearchBudget() {
        company.researchInvestment -= 10000;
        if (company.researchInvestment < 0)
            company.researchInvestment = 0;
    }

    public UILabel salaries;
    public UILabel rent;
    public UILabel inf;
    public UILabel numWorkerLocations;
    public UILabel currentRevenue;
    public UILabel currentProfit;
    public UILabel pastRevenue;
    public UILabel pastProfit;
    public UILabel researchBudget;
}
