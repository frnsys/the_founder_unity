using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageCommunications : UIFullScreenPager {
    private GameManager gm;
    private Company company;

    public GameObject promoPrefab;
    public UILabel promoLabel;
    public UITexture promoIcon;
    public UIProgressBar progress;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        LoadPromos();
    }

    private void LoadPromos() {
        ClearGrid();
        foreach (Promo p in gm.unlocked.promos.Where(p => p != company.developingPromo)) {
            GameObject promoItem = NGUITools.AddChild(grid.gameObject, promoPrefab);
            promoItem.GetComponent<UIPromo>().promo = p;
        }
        Adjust();
    }

    void Update() {
        if (company.developingPromo == null) {
            promoLabel.text = "No current promotion.";
            promoIcon.mainTexture = null;
            progress.value = 0;
        } else {
            promoLabel.text = company.developingPromo.name;
            promoIcon.mainTexture = company.developingPromo.icon;
            progress.value = company.developingPromo.progress;
        }
    }

    public GameObject historyPrefab;
    public void OpenHistory() {
        UIManager.Instance.CloseAndOpenPopup(historyPrefab);
    }
}
