using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISelectWorkerPopup : UIPopup {
    public GameObject window;
    public UILabel titleLabel;
    public UITexture overlay;
    public UICenteredGrid grid;
    public UIScrollView scrollView;
    public GameObject workerItemPrefab;
    private Worker selected;
    private GameObject selectedItem;
    private Action<Worker> onConfirm;
    private Dictionary<int, Worker> workerMap;

    public void SetData(string title, IEnumerable<Worker> workers, Action<Worker> confirm) {
        titleLabel.text = title;
        onConfirm = confirm;
        workerMap = new Dictionary<int, Worker>();

        foreach (Worker w in workers) {
            GameObject item = NGUITools.AddChild(grid.gameObject, workerItemPrefab);
            item.GetComponent<UIDragScrollView>().scrollView = scrollView;
            UIEventListener.Get(item).onClick += Select;
            workerMap[item.GetInstanceID()] = w;

            item.transform.Find("Name").GetComponent<UILabel>().text = w.name;
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
        selected = workerMap[obj.GetInstanceID()];
    }

    // ---

    private float overlayAlpha = 0.3f;

    void OnEnable() {
        Show();
    }

    public void Show() {
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
        base.Show(window);
    }

    public void Close_() {
        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        Hide(window);
    }

    private IEnumerator FadeOverlay(float from, float to) {
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
