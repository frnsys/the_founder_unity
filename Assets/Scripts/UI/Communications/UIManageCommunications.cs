using UnityEngine;
using System;
using System.Collections;

public class UIManageCommunications : MonoBehaviour {
    private Company playerCompany;

    public UILabel promoLabel;
    public UILabel investmentLabel;
    public UILabel czarLabel;
    public GameObject newPromoButton;
    public UIProgressBar progress;

    void OnEnable() {
        playerCompany = GameManager.Instance.playerCompany;
    }

    void Update() {
        if (playerCompany.developingPromo == null) {
            promoLabel.text = "";
            newPromoButton.SetActive(true);
            progress.value = 0;
        } else {
            promoLabel.text = playerCompany.developingPromo.name;
            newPromoButton.SetActive(false);
            progress.value = playerCompany.developingPromo.progress;
        }
        if (playerCompany.OpinionCzar == null)
            czarLabel.text = "No Director of Communications appointed.";
        else
            czarLabel.text = playerCompany.OpinionCzar.name;
        //investmentLabel.text = "$" + playerCompany.researchInvestment;
    }

    public void SelectNewPromo() {
        Action<Promo> select = delegate(Promo p) {
            playerCompany.StartPromo(p);
        };

        UIManager.Instance.PromoSelectionPopup(select);
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
            playerCompany.OpinionCzar = w;
        };
        UIManager.Instance.WorkerSelectionPopup("Appoint a new Head of Communications", select, null, playerCompany.OpinionCzar);
    }
}
