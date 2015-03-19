using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageResearch : UIFullScreenPager {
    public GameObject techPrefab;
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

    private void ReloadTechs(Technology t) {
        LoadTechs();
    }
}
