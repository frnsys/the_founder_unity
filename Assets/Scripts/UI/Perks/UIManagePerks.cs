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
        // Show perks where at least the first upgrade is available.
        foreach (Perk p in gm.unlocked.perks.Where(pk => pk.upgrades[0].Available(gm.playerCompany))) {
            GameObject perkItem = NGUITools.AddChild(grid.gameObject, perkPrefab);
            perkItem.GetComponent<UIPerk>().perk = new APerk(p);
        }
        Adjust();
    }
}


