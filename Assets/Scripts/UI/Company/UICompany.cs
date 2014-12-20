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

    public void ShowManageView() {
        manageView.SetActive(true);
        overviewView.SetActive(false);
        manageButton.defaultColor = activeColor;
        overviewButton.defaultColor = inactiveColor;
    }
    public void ShowOverviewView() {
        manageView.SetActive(false);
        overviewView.SetActive(true);
        manageButton.defaultColor = inactiveColor;
        overviewButton.defaultColor = activeColor;
    }

}
