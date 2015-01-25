using UnityEngine;
using System;
using System.Collections;

public class UIResearchOverview : MonoBehaviour {
    private Company playerCompany;
    private ResearchManager rm;

    public UILabel techLabel;
    public UILabel investmentLabel;
    public UILabel czarLabel;
    public UITexture techIcon;
    public UIProgressBar progress;

    void OnEnable() {
        playerCompany = GameManager.Instance.playerCompany;
        rm = GameManager.Instance.researchManager;
    }

    void Update() {
        if (rm.technology == null) {
            techLabel.text = "No current research.";
            techIcon.mainTexture = null;
        } else {
            techLabel.text = rm.technology.name;
            techIcon.mainTexture = rm.technology.icon;
        }
        if (playerCompany.ResearchCzar == null)
            czarLabel.text = "No Head of Research appointed.";
        else
            czarLabel.text = playerCompany.ResearchCzar.name;
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

    public void AppointCzar() {
        Action<Worker> select = delegate(Worker w) {
            playerCompany.ResearchCzar = w;
        };
        UIManager.Instance.WorkerSelectionPopup("Appoint a new Head of Research", select, null, playerCompany.ResearchCzar);
    }

    public void SelectTechnology() {
        Action<Technology> select = delegate(Technology t) {
            if (rm.technology != null && rm.technology != t) {
                UIManager.Instance.Confirm("Are you sure want to change what you're researching? You will lose accumulated progress for the current technology.", delegate() {
                    rm.BeginResearch(t);
                }, null);
            } else {
                rm.BeginResearch(t);
            }
        };
        UIManager.Instance.ResearchSelectionPopup("Select something to research", rm.AvailableTechnologies(), select, rm.technology);
    }
}
