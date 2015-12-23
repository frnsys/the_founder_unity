using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInterlude : MonoBehaviour {
    public UIGrid grid;
    public UILabel yearLabel;
    public UILabel ageLabel;
    public UILabel companyAgeLabel;
    public UILabel cashLabel;
    public UILabel boardLabel;
    public UILabel profitLabel;
    public UILabel marketingLabel;
    public UILabel designLabel;
    public UILabel engineeringLabel;
    public UILabel opinionLabel;
    public UILabel productivityLabel;
    public UILabel hypeLabel;

    private Company company;

    void OnEnable() {
        GameManager gm = GameManager.Instance;
        company = gm.playerCompany;
        yearLabel.text = gm.year.ToString();
        ageLabel.text = gm.age.ToString();
        companyAgeLabel.text = gm.companyAge.ToString();
        boardLabel.text = gm.boardStatus.ToString();
        profitLabel.text = string.Format("{0:C0}", gm.profitTarget);
    }

    void Update() {
        cashLabel.text = string.Format("{0:C0}", company.cash.value);
        marketingLabel.text = string.Format(":MARKETING: {0:F0}", company.charisma);
        designLabel.text = string.Format(":DESIGN: {0:F0}", company.creativity);
        engineeringLabel.text = string.Format(":ENGINEERING: {0:F0}", company.cleverness);
        productivityLabel.text = string.Format(":PRODUCTIVITY: {0:F0}", company.productivity);
        hypeLabel.text = string.Format(":HYPE: {0:F0}", company.hype);

        string emo = "OUTRAGE";
        if (company.opinion >= 0) {
            emo = "GOODWILL";
        }
        opinionLabel.text = string.Format(":{0}: {1:F0}", emo, company.opinion);
    }

    public void Hide(string item) {
        GetItem(item).transform.parent.gameObject.SetActive(false);
        grid.Reposition();
    }

    public void Show(string item) {
        GetItem(item).transform.parent.gameObject.SetActive(true);
        grid.Reposition();
    }

    private UILabel GetItem(string item) {
        switch (item) {
            case "Hype":
                return hypeLabel;
                break;
            case "Opinion":
                return opinionLabel;
                break;
            case "Board":
                return boardLabel;
                break;
            case "Profit":
                return profitLabel;
                break;
        }
        return null;
    }
}
