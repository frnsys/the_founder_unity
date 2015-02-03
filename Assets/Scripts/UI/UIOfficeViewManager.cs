using UnityEngine;
using System;
using System.Collections;

public class UIOfficeViewManager : Singleton<UIOfficeViewManager> {
    private GameData data;
    private Company playerCompany;
    public Camera officeCamera;
    public UILabel viewLabel;
    public UITexture background;

    public OfficeArea labs;
    public OfficeArea comms;
    public OfficeArea market;

    [System.Serializable]
    public struct OfficeView {
        public string name;
        public Vector3 position;
        public Color color;
    }
    public OfficeView[] officeViews;
    private int current;
    public OfficeView currentView {
        get { return officeViews[current]; }
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        current = 0;
    }

    public void Load(GameData d) {
        data = d;
        playerCompany = d.company;

        // Setup which office areas are accessible.
        labs.accessible = d.LabsAccessible;
        comms.accessible = d.CommsAccessible;
        market.accessible = d.MarketAccessible;
    }

    public void BuyLabs() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.LabsAccessible = true;
            labs.accessible = true;
        }
    }

    public void BuyComms() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.CommsAccessible = true;
            comms.accessible = true;
        }
    }

    public void BuyMarket() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.MarketAccessible = true;
            market.accessible = true;
        }
    }

    public void NextView() {
        if (current == officeViews.Length - 1)
            current = 0;
        else
            current++;

        SetView(current);
        StopCoroutine("ChangeView");
        StartCoroutine("ChangeView");
    }

    public void PrevView() {
        if (current == 0)
            current = officeViews.Length - 1;
        else
            current--;

        SetView(current);
        StopCoroutine("ChangeView");
        StartCoroutine("ChangeView");
    }

    void SetView(int idx) {
        current = idx;
        viewLabel.text = currentView.name;
        background.color = currentView.color;
    }

    IEnumerator ChangeView() {
        float step = 0.05f;
        Vector3 fromPos = officeCamera.transform.position;
        Vector3 toPos = officeViews[current].position;

        for (float f = 0f; f <= 1f + step; f += step) {
            // SmoothStep gives us a bit of easing.
            officeCamera.transform.position = Vector3.Lerp(fromPos, toPos, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    // Check what the closest office view to the current camera position is.
    public void CheckView() {
        Vector3 pos = officeCamera.transform.position;
        int minIdx = 0;
        float minDist = Vector3.Distance(pos, officeViews[0].position);
        for (int i=1; i < officeViews.Length - 1; i++) {
            float dist = Vector3.Distance(pos, officeViews[i].position);
            if (dist < minDist) {
                minIdx = i;
                minDist = dist;
            }
        }
        SetView(minIdx);
    }

}
