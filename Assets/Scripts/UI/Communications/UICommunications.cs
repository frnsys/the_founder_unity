using UnityEngine;
using System;
using System.Collections;

public class UICommunications : UIWindow {
    public GameObject historyView;
    public GameObject manageView;
    public UIButton historyButton;
    public UIButton manageButton;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    public void ShowHistoryView() {
        historyView.SetActive(true);
        manageView.SetActive(false);
        historyButton.defaultColor = activeColor;
        manageButton.defaultColor = inactiveColor;
    }
    public void ShowManageView() {
        historyView.SetActive(false);
        manageView.SetActive(true);
        historyButton.defaultColor = inactiveColor;
        manageButton.defaultColor = activeColor;
    }
}
