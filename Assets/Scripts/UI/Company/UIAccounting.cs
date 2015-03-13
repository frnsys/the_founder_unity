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
        salaries.text = string.Format("Salaries: [c][EF4542]{0:C0}[-][/c]/month", monthlySalaries);

        float monthlyRent = company.locations.Skip(1).Sum(l => l.cost);
        float monthlyInf = company.infrastructure.cost;
        rent.text = string.Format("Rent: [c][EF4542]{0:C0}[-][/c]/month", monthlyRent);
        inf.text = string.Format("Infrastructure: [c][EF4542]{0:C0}[-][/c]/month", monthlyInf);

        numWorkerLocations.text = string.Format("[c][6A53F7]{0}[-][/c] employees across [c][6A53F7]{1}[-][/c] locations ([c][6A53F7]{2}[-][/c] employees at HQ)", company.employeesAcrossLocations, company.locations.Count, company.workers.Count);

        PerformanceDict lastAnnual = company.lastAnnualPerformance;
        float revs = 0;
        float cost = 0;
        if (lastAnnual != null) {
            revs = lastAnnual["Annual Revenue"];
            cost = lastAnnual["Annual Costs"];
        }
        pastRevenue.text = string.Format("Revenue: [c][6A53F7]{0:C0}[-][/c]", revs);
        pastProfit.text = string.Format("Profit: [c][6A53F7]{0:C0}[-][/c]", revs-cost);

        string economy = "[c][20D060]healthy[-][/c]";
        switch (gm.economy) {
            case Economy.Depression:
                economy = "[c][EF4542]in a depression[-][/c]";
                break;
            case Economy.Recession:
                economy = "[c][EF4542]in a recession[-][/c]";
                break;
            case Economy.Expansion:
                economy = "[c][20D060]booming[-][/c]";
                break;
        }
        economicHealth.text = string.Format("The economy is {0}.", economy);
        lifetimeRevenue.text = string.Format("Lifetime Revenue: [c][6A53F7]{0:C0}[-][/c]", company.lifetimeRevenue);
        totalMarketShare.text = string.Format("Global Coverage: [c][6A53F7]{0:F2}%[-][/c]", company.marketSharePercent * 100);
        deathToll.text = string.Format("Deaths Caused by Products: [c][EF4542]{0}[-][/c]", company.deathToll);
        debtOwned.text = string.Format("World Debt Owned by Company: [c][EF4542]{0}[-][/c]", company.debtOwned);
        pollution.text = string.Format("Pollution Emitted: [c][EF4542]{0:0}[-][/c] metric tons", company.pollution);
        spendingRate.text = string.Format("Consumer Spending Rate: [c][20D060]{0:F2}x[-][/c]", gm.spendingMultiplier);
        taxesAvoided.text = string.Format("Taxes Avoided: [c][20D060]{0:C0}[-][/c]", company.taxesAvoided);
        averageIncome.text = string.Format("Global Average Income: [c][6A53F7]{0:C0}/yr[-][/c]", gm.wageMultiplier * 60000);
        forgettingRate.text = string.Format("Public Forgetting Rate: [c][20D060]{0:F2}x[-][/c]", gm.forgettingRate);
    }

    void Update() {
        float revs = company.annualRevenue;
        float cost = company.annualCosts;
        currentRevenue.text = string.Format("Revenue: [c][6A53F7]{0:C0}[-][/c]", revs);
        currentProfit.text = string.Format("Profit: [c][6A53F7]{0:C0}[-][/c]", revs-cost);
        lifetimeRevenue.text = string.Format("Lifetime Revenue: [c][6A53F7]{0:C0}[-][/c]", company.lifetimeRevenue);
        totalMarketShare.text = string.Format("Global Coverage: [c][6A53F7]{0:F2}%[-][/c]", company.marketSharePercent * 100);
        deathToll.text = string.Format("Deaths Caused by Products: [c][EF4542]{0}[-][/c]", company.deathToll);
        debtOwned.text = string.Format("World Debt Owned by Company: [c][EF4542]{0}[-][/c]", company.debtOwned);
        pollution.text = string.Format("Pollution Emitted: [c][EF4542]{0:0}[-][/c] metric tons", company.pollution);
        taxesAvoided.text = string.Format("Taxes Avoided: [c][20D060]{0:C0}[-][/c]", company.taxesAvoided);
        averageIncome.text = string.Format("Global Average Income: [c][6A53F7]{0:C0}/yr[-][/c]", gm.wageMultiplier * 60000);
        forgettingRate.text = string.Format("Public Forgetting Rate: [c][20D060]{0:F2}x[-][/c]", gm.forgettingRate);
    }


    public UILabel salaries;
    public UILabel rent;
    public UILabel inf;
    public UILabel numWorkerLocations;
    public UILabel currentRevenue;
    public UILabel currentProfit;
    public UILabel pastRevenue;
    public UILabel pastProfit;
    public UILabel lifetimeRevenue;
    public UILabel totalMarketShare;
    public UILabel deathToll;
    public UILabel debtOwned;
    public UILabel pollution;
    public UILabel economicHealth;
    public UILabel spendingRate;
    public UILabel taxesAvoided;
    public UILabel averageIncome;
    public UILabel forgettingRate;
}
