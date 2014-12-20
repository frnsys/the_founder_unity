using UnityEngine;
using System;
using System.Collections;

public class UIResearchOverview : MonoBehaviour {
    private Company playerCompany;
    private ResearchManager rm;

    public UILabel techLabel;
    public UILabel investmentLabel;
    public UIProgressBar progress;

    void OnEnable() {
        playerCompany = GameManager.Instance.playerCompany;
        rm = GameManager.Instance.researchManager;
    }

    void Update() {
        if (rm.technology == null) {
            techLabel.text = "No current research.";
        } else {
            techLabel.text = rm.technology.name;
        }
        investmentLabel.text = "$" + playerCompany.researchInvestment;
        progress.value = rm.progress;
    }

    public void IncreaseInvestment() {
        playerCompany.researchInvestment += 10000;
    }

    public void PlusIncreaseInvestment() {
        playerCompany.researchInvestment += 1000000;
    }

    public void DecreaseInvestment() {
        playerCompany.researchInvestment -= 10000;
        if (playerCompany.researchInvestment < 0)
            playerCompany.researchInvestment = 0;
    }

    public void PlusDecreaseInvestment() {
        playerCompany.researchInvestment -= 1000000;
        if (playerCompany.researchInvestment < 0)
            playerCompany.researchInvestment = 0;
    }
}
