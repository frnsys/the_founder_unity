using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIPurchasePerks : MonoBehaviour {
    private GameManager gm;

    public GameObject shopItemPrefab;
    public GameObject perkDetailPrefab;
    public UISimpleGrid grid;

    public List<Perk> displayedPerks = new List<Perk>();

    void OnEnable() {
        gm = GameManager.Instance;
    }

    void Update() {
        foreach (Perk perk in gm.unlocked.perks) {
            if (!displayedPerks.Contains(perk)) {
                // Only show perks which are not already owned by the company.
                if (Perk.Find(perk, gm.playerCompany.perks) == null) {
                    GameObject itemItem = NGUITools.AddChild(grid.gameObject, shopItemPrefab);
                    itemItem.GetComponent<UIMarketItem>().perk = perk;
                    UIEventListener.Get(itemItem).onClick += ShowItemDetail;

                    displayedPerks.Add(perk);
                }
            } else {
                // If a displayed perk now does belong to a company (e.g. it's been bought),
                // remove it.
                if (Perk.Find(perk, gm.playerCompany.perks) != null) {
                    int childIdx = displayedPerks.IndexOf(perk);
                    NGUITools.Destroy(grid.transform.GetChild(childIdx).gameObject);
                    displayedPerks.Remove(perk);
                }
            }
        }
        grid.Reposition();
    }

    public void ShowItemDetail(GameObject obj) {
        GameObject itemDetailPopup = NGUITools.AddChild(transform.parent.gameObject, perkDetailPrefab);
        itemDetailPopup.GetComponent<UIMarketItemDetail>().perk = obj.GetComponent<UIMarketItem>().perk;
    }
}


