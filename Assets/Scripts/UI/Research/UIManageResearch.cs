using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageResearch : UIFullScreenPager {
    public GameObject techPrefab;
    public UILabel researchBudgetLabel;
    private Company company;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        LoadTechs();

        Company.ResearchCompleted += ReloadTechs;
    }

    void OnDisable() {
        Company.ResearchCompleted -= ReloadTechs;
    }

    private void LoadTechs() {
        ClearGrid();
        foreach (Technology t in Technology.LoadAll().Where(t => t.isAvailable(company))) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            techItem.GetComponent<UITechnology>().technology = t;
        }
        foreach (Technology t in company.technologies) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            UITechnology uit = techItem.GetComponent<UITechnology>();
            uit.technology = t;
            uit.researched = true;
        }
        Adjust();
    }

    void Update() {
        researchBudgetLabel.text = string.Format("{0:C0}", company.researchInvestment);
    }

    private void ReloadTechs(Technology t) {
        LoadTechs();
    }

    public void IncreaseResearchBudget() {
        company.researchInvestment += 10000;
    }

    public void DecreaseResearchBudget() {
        company.researchInvestment -= 10000;
        if (company.researchInvestment < 0)
            company.researchInvestment = 0;
    }
}
