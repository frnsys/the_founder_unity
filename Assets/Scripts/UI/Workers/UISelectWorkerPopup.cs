using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISelectWorkerPopup : UISelectPopup {
    public UILabel titleLabel;
    private Worker selected;
    private GameObject selectedItem;
    private Action<Worker> onConfirm;
    private Dictionary<int, Worker> workerMap;

    public void SetData(string title, IEnumerable<Worker> workers, Action<Worker> confirm, Worker selected) {
        titleLabel.text = title;
        onConfirm = confirm;
        workerMap = new Dictionary<int, Worker>();

        foreach (Worker w in workers) {
            GameObject item = NGUITools.AddChild(grid.gameObject, itemPrefab);
            item.GetComponent<UIDragScrollView>().scrollView = scrollView;
            UIEventListener.Get(item).onClick += Select;
            workerMap[item.GetInstanceID()] = w;

            item.transform.Find("Name").GetComponent<UILabel>().text = w.name;

            if (w == selected) {
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
            selected = workerMap[obj.GetInstanceID()];

        // Otherwise, de-select it.
        } else {
            selected = null;
        }
    }
}
