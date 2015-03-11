using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIRecruiting : UIFullScreenPager {
    private GameManager gm;
    public GameObject recruitmentPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadRecruitments();
    }

    private void LoadRecruitments() {
        ClearGrid();
        foreach (Recruitment p in gm.unlocked.recruitments) {
            GameObject recruitmentItem = NGUITools.AddChild(grid.gameObject, recruitmentPrefab);
            recruitmentItem.GetComponent<UIRecruitment>().recruitment = p;
        }
        Adjust();
    }
}
