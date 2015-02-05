using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIAcquisitions : UIFullScreenPager {
    private Company company;
    private GameManager gm;

    public GameObject companyPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        LoadCompanies();
    }

    private void LoadCompanies() {
        ClearGrid();
        foreach (MiniCompany c in gm.unlocked.companies) {
            GameObject companyItem = NGUITools.AddChild(grid.gameObject, companyPrefab);
            companyItem.GetComponent<UIMiniCompany>().company = c;
        }
        Adjust();
    }
}


