using UnityEngine;
using System;
using System.Collections;

public class UIResearchArchives : UIFullScreenPager {
    public GameObject techPrefab;
    private Company playerCompany;

    void OnEnable() {
        ClearGrid();
        playerCompany = GameManager.Instance.playerCompany;
        foreach (Technology t in playerCompany.technologies) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            techItem.GetComponent<UITechnologyItem>().technology = t;
        }
        Adjust();
    }
}
