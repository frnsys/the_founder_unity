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
        salaries.text = string.Format("{0:C0} per month in salaries", monthlySalaries);

        float monthlyRent = 0;
        float monthlyInf = 0;
        foreach (Location loc in company.locations) {
            monthlyRent += loc.cost;

            // We have to add up location infrastructure cost in this way,
            // so we incorporate the cost of the infrastructure for the location.
            monthlyInf += loc.infrastructure.cost;
        }
        rent.text = string.Format("{0:C0} per month in rent", monthlyRent);
        inf.text = string.Format("{0:C0} per month in infrastructure costs", monthlyInf);

        numWorkerLocations.text = string.Format("{0} employees across {1} locations ({2} employees at HQ)", company.employeesAcrossLocations, company.locations.Count, company.workers.Count);

        PerformanceDict lastQuarter = company.lastQuarterPerformance;
        if (lastQuarter == null) {
            pastRevenue.text = "Revenue: $0";
            pastProfit.text = "Profit: $0";
        } else {
            float revs = lastQuarter["Quarterly Revenue"];
            float cost = lastQuarter["Quarterly Costs"];
            pastRevenue.text = string.Format("Revenue: {0:C0}", revs);
            pastProfit.text = string.Format("Profit: {0:C0}", revs-cost);
        }
        lifetimeRevenue.text = string.Format("Lifetime Revenue: {0:C0}", company.lifetimeRevenue);
        totalMarketShare.text = string.Format("Total Market Share: {0:F2}%", company.totalMarketShare);
        deathToll.text = string.Format("Deaths Caused by Products: {0}", company.deathToll);
        debtOwned.text = string.Format("World Debt Owned by Company: {0}", company.debtOwned);
    }

    void Update() {
        float revs = company.quarterRevenue;
        float cost = company.quarterCosts;
        currentRevenue.text = string.Format("Revenue: {0:C0}", revs);
        currentProfit.text = string.Format("Profit: {0:C0}", revs-cost);
        researchBudget.text = company.researchInvestment.ToString();
        lifetimeRevenue.text = string.Format("Lifetime Revenue: {0:C0}", company.lifetimeRevenue);
        totalMarketShare.text = string.Format("Total Market Share: {0:F2}%", company.totalMarketShare);
        deathToll.text = string.Format("Deaths Caused by Products: {0}", company.deathToll);
        debtOwned.text = string.Format("World Debt Owned by Company: {0}", company.debtOwned);
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
    public UILabel lifetimeRevenue;
    public UILabel totalMarketShare;
    public UILabel deathToll;
    public UILabel debtOwned;
}
