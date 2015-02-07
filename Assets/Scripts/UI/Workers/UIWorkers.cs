using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWorkers : UIWindow {
    public GameObject hireWorkers;
    public GameObject manageWorkers;
    public UIButton hireWorkersButton;
    public UIButton manageWorkersButton;
    public UILabel wpLabel;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);
    private Company playerCompany;

    void Start() {
        playerCompany = GameManager.Instance.playerCompany;
        currentScreen_ = manageWorkers;
    }

    void Update() {
        wpLabel.text = string.Format("{0}/{1} workers", playerCompany.workers.Count, playerCompany.sizeLimit);
    }

    public void ShowHireWorkers() {
        SelectTab(hireWorkers);
        hireWorkersButton.defaultColor = activeColor;
        manageWorkersButton.defaultColor = inactiveColor;
    }

    public void ShowManageWorkers() {
        SelectTab(manageWorkers);
        hireWorkersButton.defaultColor = inactiveColor;
        manageWorkersButton.defaultColor = activeColor;
    }
}
