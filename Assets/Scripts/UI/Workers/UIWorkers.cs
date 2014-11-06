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

    void Update() {
        wpLabel.text = GameManager.Instance.playerCompany.remainingSpace.ToString() + " available capacity";
    }

    public void ShowHireWorkers() {
        hireWorkers.SetActive(true);
        hireWorkersButton.defaultColor = activeColor;

        manageWorkers.SetActive(false);
        manageWorkersButton.defaultColor = inactiveColor;
    }

    public void ShowManageWorkers() {
        hireWorkers.SetActive(false);
        hireWorkersButton.defaultColor = inactiveColor;

        manageWorkers.SetActive(true);
        manageWorkersButton.defaultColor = activeColor;
    }
}
