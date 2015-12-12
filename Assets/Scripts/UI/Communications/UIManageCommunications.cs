using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UIManageCommunications : MonoBehaviour {
    public GameObject promoPrefab;
    public UISimpleGrid grid;
    public UIScrollView scrollView;
    public List<UIPromo> promos;

    void OnEnable() {
        LoadPromos();
    }

    private void LoadPromos() {
        int i = 0;
        foreach (Promo p in Promo.LoadAll().OrderBy(p => p.cost)) {
            GameObject promoItem = NGUITools.AddChild(grid.gameObject, promoPrefab);
            UIPromo uip = promoItem.GetComponent<UIPromo>();
            promoItem.GetComponent<UIDragScrollView>().scrollView = scrollView;
            uip.promo = p;
            uip.stars = i;
            i++;

            if (!GameManager.Instance.playerCompany.promos.Contains(p.id)) {
                uip.Lock();
            }
            promos.Add(uip);
        }
        grid.Reposition();
    }

    void Update() {
        foreach (UIPromo uip in promos) {
            if (!GameManager.Instance.playerCompany.promos.Contains(uip.promo.id)) {
                uip.Lock();
            } else {
                uip.Unlock();
            }
        }
    }

    public GameObject historyPrefab;
    public void OpenHistory() {
        UIManager.Instance.CloseAndOpenPopup(historyPrefab);
    }
}
