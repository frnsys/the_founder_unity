using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIResearchArchive : UIFullScreenPager {
    private Company company;
    private ResearchManager rm;

    public GameObject techPrefab;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        rm = GameManager.Instance.researchManager;
        LoadTechs();
    }

    private void LoadTechs() {
        ClearGrid();
        foreach (Technology t in company.technologies) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            techItem.GetComponent<UITechnology>().technology = t;
        }
        Adjust();
    }
}
