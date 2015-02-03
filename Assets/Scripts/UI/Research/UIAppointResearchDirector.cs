using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIAppointResearchDirector : UIFullScreenPager {
    private Company company;

    public GameObject candidatePrefab;
    public GameObject currentDirector;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        LoadCandidates();
    }

    private void LoadCandidates() {
        ClearGrid();
        foreach (Worker w in company.workers.Where(w => w != company.ResearchCzar && w != company.OpinionCzar)) {
            GameObject workerItem = NGUITools.AddChild(grid.gameObject, candidatePrefab);
            workerItem.GetComponent<UIDirectorCandidate>().worker = w;
        }
        Adjust();
    }

    public void DismissCzar() {
        UIManager.Instance.Confirm("Are you sure you want to dismiss your Director of Research?", delegate() {
            company.ResearchCzar = null;
        }, null);
    }

    void Update() {
        if (company.ResearchCzar == null) {
            currentDirector.SetActive(false);
        } else {
            currentDirector.SetActive(true);
            GetComponent<UIDirectorCandidate>().worker = company.ResearchCzar;
        }
    }
}
