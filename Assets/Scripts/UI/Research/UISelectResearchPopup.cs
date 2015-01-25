using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISelectResearchPopup : UISelectPopup {
    public UILabel titleLabel;
    private Technology selected;
    private GameObject selectedItem;
    private Action<Technology> onConfirm;
    private Dictionary<int, Technology> techMap;

    public void SetData(string title, IEnumerable<Technology> techs, Action<Technology> confirm, Technology selected) {
        titleLabel.text = title;
        onConfirm = confirm;
        techMap = new Dictionary<int, Technology>();

        foreach (Technology t in techs) {
            GameObject item = NGUITools.AddChild(grid.gameObject, itemPrefab);
            item.GetComponent<UIDragScrollView>().scrollView = scrollView;
            UIEventListener.Get(item).onClick += Select;
            techMap[item.GetInstanceID()] = t;

            item.transform.Find("Name").GetComponent<UILabel>().text = t.name;

            if (t == selected) {
                Select(item);
            }
        }
        grid.Reposition();
    }

    public void Confirm() {
        onConfirm(selected);
        Close_();
    }

    public void Select(GameObject obj) {
        // Clear existing selection.
        if (selectedItem != null)
            selectedItem.transform.Find("Highlight").gameObject.SetActive(false);

        // Select _only_ if the new item is different from the old one.
        if (selectedItem != obj) {
            obj.transform.Find("Highlight").gameObject.SetActive(true);
            selectedItem = obj;
            selected = techMap[obj.GetInstanceID()];

        // Otherwise, de-select it.
        } else {
            selected = null;
        }
    }
}
