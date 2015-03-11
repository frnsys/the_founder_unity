using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UIRecruiting : UIFullScreenPager {
    public GameObject recruitmentPrefab;

    void OnEnable() {
        LoadRecruitments();
    }

    private void LoadRecruitments() {
        ClearGrid();
        foreach (Recruitment p in Recruitment.LoadAll().Where(r => !r.robots || GameManager.Instance.automation)) {
            GameObject recruitmentItem = NGUITools.AddChild(grid.gameObject, recruitmentPrefab);
            recruitmentItem.GetComponent<UIRecruitment>().recruitment = p;
        }
        Adjust();
    }
}
