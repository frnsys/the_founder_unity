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
        public UILabel label;
        public AICompany company;

        public CompanyInfo(AICompany company_, UILabel label_) {
            label = label_;
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
            CompanyInfo ci = new CompanyInfo(ac, go.GetComponent<UILabel>());
            companyInfos.Add(ci);
        }
        companiesGrid.Reposition();
    }

    void Update() {
        foreach (CompanyInfo ci in companyInfos) {
            ci.label.text = ci.company.name + ": $" + ci.company.cash.value.ToString() + " | #P" + ci.company.products.Count.ToString();
        }
    }
}
