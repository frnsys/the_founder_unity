using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageCommunications : UIFullScreenPager {
    private GameManager gm;

    public GameObject promoPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadPromos();
    }

    private void LoadPromos() {
        ClearGrid();
        foreach (Promo p in gm.unlocked.promos) {
            GameObject promoItem = NGUITools.AddChild(grid.gameObject, promoPrefab);
            promoItem.GetComponent<UIPromo>().promo = p;
        }
        Adjust();
    }

    public GameObject historyPrefab;
    public void OpenHistory() {
        UIManager.Instance.CloseAndOpenPopup(historyPrefab);
    }
}
