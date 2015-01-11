/*
 * The Simulator plays through an AI-only game.
 * We can use this to check AI performance, check game balance,
 * and check that things generally work ok.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Simulator : MonoBehaviour {
    private GameObject gameObj;
    private GameData gd;
    private GameManager gm;

    public UIGrid companiesGrid;
    public GameObject companyPrefab;
    private List<CompanyInfo> companyInfos;

    private struct CompanyInfo {
        public Transform t;
        public AICompany company;

        public CompanyInfo(AICompany company_, Transform t_) {
            t = t_;
            company = company_;
        }
    }

    void Start() {
        gameObj = new GameObject("Game Manager");
        gm = gameObj.AddComponent<GameManager>();
        gd = GameData.New("DEFAULTCORP");
        gm.Load(gd);

        // Go FAST!
        Time.timeScale = 100;

        companyInfos = new List<CompanyInfo>();
        foreach (AICompany ac in gd.otherCompanies) {
            GameObject go = NGUITools.AddChild(companiesGrid.gameObject, companyPrefab);
            CompanyInfo ci = new CompanyInfo(ac, go.transform);
            companyInfos.Add(ci);
        }
        companiesGrid.Reposition();
    }

    void Update() {
        foreach (CompanyInfo ci in companyInfos) {
            SetText(ci.t, "Name", ci.company.name);
            SetText(ci.t, "Cash", "$" + ci.company.cash.value.ToString());
            SetText(ci.t, "Active Products", "In-Market Products: " + ci.company.activeProducts.Count.ToString());
            SetText(ci.t, "Infrastructure Capacity", "Inf Cap: " + ci.company.infrastructureCapacity.ToString());
            SetText(ci.t, "Available Infrastructure", "Av Inf: " + ci.company.availableInfrastructure.ToString());
            SetText(ci.t, "Lifetime Revenue", "Lifetime Rev: $" + ci.company.lifetimeRevenue.ToString());
            SetText(ci.t, "Locations", "Locations: " + ci.company.locations.Count.ToString());
        }
    }

    void SetText(Transform t, string name, string text) {
        t.Find(name).GetComponent<UILabel>().text = text;
    }
}
