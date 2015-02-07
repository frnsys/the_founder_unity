using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManagePerks : UIFullScreenPager {
    private GameManager gm;
    public GameObject perkPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadPerks();
    }

    private void LoadPerks() {
        ClearGrid();
        foreach (Perk p in gm.unlocked.perks) {
            GameObject perkItem = NGUITools.AddChild(grid.gameObject, perkPrefab);
            perkItem.GetComponent<UIPerk>().perk = p;
        }
        Adjust();
    }
}


