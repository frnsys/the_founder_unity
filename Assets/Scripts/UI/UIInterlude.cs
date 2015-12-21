using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInterlude : MonoBehaviour {
    public UILabel yearLabel;
    public UILabel ageLabel;
    public UILabel companyAgeLabel;
    public UILabel cashLabel;
    public UILabel boardLabel;
    public UILabel profitLabel;
    public UILabel marketingLabel;
    public UILabel designLabel;
    public UILabel engineeringLabel;
    public UILabel goodwillLabel;
    public UILabel productivityLabel;
    public UILabel happinessLabel;

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
        marketingLabel.text = string.Format(":MARKETING: {0:F0}", company.charisma);
        designLabel.text = string.Format(":DESIGN: {0:F0}", company.creativity);
        engineeringLabel.text = string.Format(":ENGINEERING: {0:F0}", company.cleverness);
        productivityLabel.text = string.Format(":PRODUCTIVITY: {0:F0}", company.productivity);
        happinessLabel.text = string.Format(":HAPPINESS: {0:F0}", company.happiness);
        goodwillLabel.text = string.Format(":GOODWILL: {0:F0}", company.goodwill);
    }
}
