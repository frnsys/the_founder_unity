using UnityEngine;
using System;
using System.Collections;

public class UIResearch : UIWindow {
    public GameObject archivesView;
    public GameObject overviewView;
    public UIButton archivesButton;
    public UIButton overviewButton;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    public void ShowArchivesView() {
        archivesView.SetActive(true);
        overviewView.SetActive(false);
        archivesButton.defaultColor = activeColor;
        overviewButton.defaultColor = inactiveColor;
    }
    public void ShowOverviewView() {
        archivesView.SetActive(false);
        overviewView.SetActive(true);
        archivesButton.defaultColor = inactiveColor;
        overviewButton.defaultColor = activeColor;
    }
}
