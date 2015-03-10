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
        foreach (Vertical l in gm.unlocked.verticals) {
            GameObject verticalItem = NGUITools.AddChild(grid.gameObject, verticalPrefab);
            verticalItem.GetComponent<UIVerticalItem>().vertical = l;
        }
        Adjust();
    }

}
