using UnityEngine;
using System;
using System.Collections;

public class UICompany : UIWindow {
    public GameObject manageView;
    public GameObject overviewView;
    public UIButton manageButton;
    public UIButton overviewButton;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    void Awake() {
        currentScreen_ = overviewView;
    }

    public void ShowManageView() {
        SelectTab(manageView);
        manageButton.defaultColor = activeColor;
        overviewButton.defaultColor = inactiveColor;
    }
    public void ShowOverviewView() {
        SelectTab(overviewView);
        manageButton.defaultColor = inactiveColor;
        overviewButton.defaultColor = activeColor;
    }

}
