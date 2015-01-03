using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISelectPromoPopup : UISelectPopup {
    private Promo selected;
    private GameObject selectedItem;
    private Action<Promo> onConfirm;
    private Dictionary<int, Promo> promoMap;

    public void SetData(IEnumerable<Promo> promos, Action<Promo> confirm) {
        onConfirm = confirm;
        promoMap = new Dictionary<int, Promo>();

        foreach (Promo p in promos) {
            GameObject item = NGUITools.AddChild(grid.gameObject, itemPrefab);
            item.GetComponent<UIDragScrollView>().scrollView = scrollView;
            UIEventListener.Get(item).onClick += Select;
            promoMap[item.GetInstanceID()] = p;

            item.transform.Find("Name").GetComponent<UILabel>().text = p.name;
        }
        grid.Reposition();
    }

    public void Confirm() {
        if (selected != null)
            onConfirm(selected);
        Close_();
    }

    public void Select(GameObject obj) {
        if (selectedItem != null)
            selectedItem.transform.Find("Highlight").gameObject.SetActive(false);
        obj.transform.Find("Highlight").gameObject.SetActive(true);
        selectedItem = obj;
        selected = promoMap[obj.GetInstanceID()];
    }
}
