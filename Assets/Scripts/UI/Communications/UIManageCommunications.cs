using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UIManageCommunications : MonoBehaviour {
    public GameObject promoPrefab;
    public UISimpleGrid grid;
    public UIScrollView scrollView;

    void OnEnable() {
        LoadPromos();
    }

    private void LoadPromos() {
        int i = 0;
        // Promos are unlocked by lifetime revenue.
        foreach (Promo p in Promo.LoadAll().OrderBy(p => p.cost).Where(p => GameManager.Instance.playerCompany.lifetimeRevenue > (p.cost - 30000) * 1.5)) {
            GameObject promoItem = NGUITools.AddChild(grid.gameObject, promoPrefab);
            UIPromo uip = promoItem.GetComponent<UIPromo>();
            promoItem.GetComponent<UIDragScrollView>().scrollView = scrollView;
            uip.promo = p;
            uip.stars = i;
            i++;
        }
        grid.Reposition();
    }

    public GameObject historyPrefab;
    public void OpenHistory() {
        UIManager.Instance.CloseAndOpenPopup(historyPrefab);
    }
}
