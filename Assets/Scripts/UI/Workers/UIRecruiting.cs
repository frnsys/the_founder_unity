using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIRecruiting : UIFullScreenPager {
    private GameManager gm;
    private Company company;

    public GameObject recruitmentPrefab;
    public UILabel recruitmentLabel;
    public UITexture recruitmentIcon;
    public UIProgressBar progress;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        LoadRecruitments();
    }

    private void LoadRecruitments() {
        ClearGrid();
        foreach (Recruitment p in gm.unlocked.recruitments.Where(p => p != company.developingRecruitment)) {
            GameObject recruitmentItem = NGUITools.AddChild(grid.gameObject, recruitmentPrefab);
            recruitmentItem.GetComponent<UIRecruitment>().recruitment = p;
        }
        Adjust();
    }

    void Update() {
        if (company.developingRecruitment == null) {
            recruitmentLabel.text = "No current recruitment strategy.";
            recruitmentIcon.mainTexture = null;
            progress.value = 0;
        } else {
            recruitmentLabel.text = company.developingRecruitment.name;
            recruitmentIcon.mainTexture = company.developingRecruitment.icon;
            progress.value = company.developingRecruitment.progress;
        }
    }
}
