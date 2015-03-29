using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageVerticals : UIFullScreenPager {
    private GameManager gm;

    public GameObject verticalPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadVerticals();
    }

    private void LoadVerticals() {
        ClearGrid();
        foreach (Vertical l in Vertical.LoadAll()) {
            GameObject verticalItem = NGUITools.AddChild(grid.gameObject, verticalPrefab);
            UIVerticalItem uvi = verticalItem.GetComponent<UIVerticalItem>();
            uvi.vertical = l;

            if (!gm.unlocked.verticals.Contains(l)) {
                uvi.Lock();
            }
        }
        Adjust();
    }

}
