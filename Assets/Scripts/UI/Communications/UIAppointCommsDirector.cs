using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIAppointCommsDirector : UIFullScreenPager {
    private Company company;
    private UIDirectorCandidate currentDirectorItem;

    public GameObject candidatePrefab;
    public GameObject currentDirector;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        currentDirectorItem = GetComponent<UIDirectorCandidate>();
        LoadCandidates();
    }

    private void LoadCandidates() {
        ClearGrid();
        foreach (Worker w in company.workers.Where(w => w != company.OpinionCzar && w != company.ResearchCzar)) {
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, candidatePrefab);
            workerItem.GetComponent<UIDirectorCandidate>().worker = w;
        }
        Adjust();
    }

    public void DismissCzar() {
        UIManager.Instance.Confirm("Are you sure you want to dismiss your Director of Communications?", delegate() {
            company.OpinionCzar = null;
        }, null);
    }

    void Update() {
        if (company.OpinionCzar == null) {
            currentDirector.SetActive(false);
        } else {
            currentDirector.SetActive(true);
            currentDirectorItem.worker = company.OpinionCzar;
        }
    }
}
