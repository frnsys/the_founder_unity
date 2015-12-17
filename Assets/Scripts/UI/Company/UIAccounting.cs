using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Smooth.Slinq;

public class UIAccounting : MonoBehaviour {
    private GameManager gm;
    private Company company;
    public UISimpleGrid grid;
    public UIScrollView scrollView;
    public UILabel salariesLabel;
    public UILabel rentLabel;
    public UILabel employeesLabel;
    public UILabel locationsLabel;
    public UILabel technologiesLabel;
    public UILabel economyLabel;
    public UILabel revenueLabel;
    public UILabel profitLabel;
    public UILabel revenueChangeLabel;
    public UILabel profitChangeLabel;
    public UILabel lifetimeRevenueLabel;
    public UILabel globalCoverageLabel;
    public UILabel deathTollLabel;
    public UILabel debtOwnedLabel;
    public UILabel pollutionLabel;
    public UILabel taxesAvoidedLabel;
    public UILabel spendingRateLabel;
    public UILabel forgettingRateLabel;
    public UILabel averageWageLabel;
    public UILabel engineeringLabel;
    public UILabel designLabel;
    public UILabel marketingLabel;
    public UILabel productivityLabel;
    public UILabel employeeSatisfactionLabel;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        grid.Reposition();
        Update();
    }

    void Update() {
        float salaries = 0;
        foreach (AWorker worker in company.workers) {
            salaries += worker.salary;
        }
        salariesLabel.text = string.Format("{0:C0}/yr", salaries);

        float monthlyRent = company.locations.Skip(1).Slinq().Select(l => l.cost).Sum();
        rentLabel.text = string.Format("{0:C0}/yr", monthlyRent * 12);

        employeesLabel.text = string.Format("{0}", company.workers.Count);
        locationsLabel.text = string.Format("{0}", company.locations.Count);
        technologiesLabel.text = string.Format("{0}", company.technologies.Count);

        string economy = "healthy";
        switch (gm.economy) {
            case Economy.Depression:
                economy = "in a depression";
                break;
            case Economy.Recession:
                economy = "in a recession";
                break;
            case Economy.Expansion:
                economy = "booming!";
                break;
        }
        economyLabel.text = economy;

        float revs = company.annualRevenue;
        float cost = company.annualCosts;
        revenueLabel.text = string.Format("{0:C0}", revs);
        profitLabel.text = string.Format("{0:C0}", revs-cost);

        //TODO
        //PerformanceDict lastAnnual = company.lastAnnualPerformance;
        //if (lastAnnual != null) {
            //revs = lastAnnual["Annual Revenue"];
            //cost = lastAnnual["Annual Costs"];
        //}
        //revenueChangeLabel.text = string.Format("Revenue: [c][6A53F7]{0:C0}[-][/c]", revs);
        //revenueChangeLabel.text = string.Format("Profit: [c][6A53F7]{0:C0}[-][/c]", revs-cost);

        lifetimeRevenueLabel.text = string.Format("{0:C0}", company.lifetimeRevenue);
        globalCoverageLabel.text = string.Format("{0:F2}%", company.marketSharePercent * 100);
        deathTollLabel.text = string.Format("{0}", company.deathToll);
        debtOwnedLabel.text = string.Format("{0}", company.debtOwned);
        pollutionLabel.text = string.Format("{0:0} metric tons", company.pollution);
        spendingRateLabel.text = string.Format("{0:F2}x", gm.spendingMultiplier);
        taxesAvoidedLabel.text = string.Format("{0:C0}", company.taxesAvoided);
        averageWageLabel.text = string.Format("{0:C0}/hr", gm.wageMultiplier * 15);
        forgettingRateLabel.text = string.Format("{0:F2}x", gm.forgettingRate);

        productivityLabel.text = string.Format("{0}", company.AggregateWorkerStat("Productivity"));
        designLabel.text = string.Format("{0}", company.AggregateWorkerStat("Design"));
        engineeringLabel.text = string.Format("{0}", company.AggregateWorkerStat("Engineering"));
        marketingLabel.text = string.Format("{0}", company.AggregateWorkerStat("Marketing"));
        employeeSatisfactionLabel.text = string.Format("{0}", company.AggregateWorkerStat("Happiness")/company.allWorkers.Count());
    }
}
