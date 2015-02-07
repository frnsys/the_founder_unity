using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIAcquisitions : UIFullScreenPager {
    private GameManager gm;
    public GameObject companyPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
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


