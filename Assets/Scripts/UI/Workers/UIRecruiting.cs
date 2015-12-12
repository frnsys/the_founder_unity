using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UIRecruiting : MonoBehaviour {
    public GameObject recruitmentPrefab;
    public UISimpleGrid grid;
    public UIScrollView scrollView;

    void OnEnable() {
        LoadRecruitments();
    }

    private void LoadRecruitments() {
        int i = 0;
        // Recruitments are unlocked by lifetime revenue.
        foreach (Recruitment p in Recruitment.LoadAll().Where(r => !r.robots || GameManager.Instance.automation).OrderBy(r => r.cost)) {
            GameObject recruitmentItem = NGUITools.AddChild(grid.gameObject, recruitmentPrefab);
            UIRecruitment uir = recruitmentItem.GetComponent<UIRecruitment>();
            recruitmentItem.GetComponent<UIDragScrollView>().scrollView = scrollView;
            uir.recruitment = p;
            uir.stars = i;
            i++;

            if (GameManager.Instance.playerCompany.lifetimeRevenue < (p.cost - 20000) * 4) {
                uir.Lock();
            }
        }
        grid.Reposition();
    }
}