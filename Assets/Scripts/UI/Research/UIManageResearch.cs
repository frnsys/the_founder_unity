using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageResearch : UIFullScreenPager {
    public GameObject techPrefab;

    void OnEnable() {
        LoadTechs();
    }

    private void LoadTechs() {
        ClearGrid();
        foreach (Technology t in Technology.LoadAll().Where(t => t.isAvailable(GameManager.Instance.playerCompany))) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            techItem.GetComponent<UITechnology>().technology = t;
        }
        Adjust();
    }
}
