using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UIRecruiting : MonoBehaviour {
    public GameObject recruitmentPrefab;
    public UISimpleGrid grid;
    public UIScrollView scrollView;
    public List<UIRecruitment> recruitments;

    void OnEnable() {
        LoadRecruitments();
    }

    private void LoadRecruitments() {
        int i = 0;
        foreach (Recruitment p in Recruitment.LoadAll().Where(r => !r.robots || GameManager.Instance.automation).OrderBy(r => r.cost)) {
            GameObject recruitmentItem = NGUITools.AddChild(grid.gameObject, recruitmentPrefab);
            UIRecruitment uir = recruitmentItem.GetComponent<UIRecruitment>();
            recruitmentItem.GetComponent<UIDragScrollView>().scrollView = scrollView;
            uir.recruitment = p;
            uir.stars = i;
            i++;

            if (!GameManager.Instance.playerCompany.recruitments.Contains(p.id)) {
                uir.Lock();
            }
            recruitments.Add(uir);
        }
        grid.Reposition();
    }

    void Update() {
        foreach (UIRecruitment uir in recruitments) {
            if (!GameManager.Instance.playerCompany.recruitments.Contains(uir.recruitment.id)) {
                uir.Lock();
            } else {
                uir.Unlock();
            }
        }
    }
}