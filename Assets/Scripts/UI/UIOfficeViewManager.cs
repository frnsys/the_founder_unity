using UnityEngine;
using System;
using System.Collections;

public class UIOfficeViewManager : Singleton<UIOfficeViewManager> {
    private GameData data;
    private Company playerCompany;

    public OfficeArea labs;
    public OfficeArea comms;
    public OfficeArea market;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void Load(GameData d) {
        data = d;
        playerCompany = d.company;

        // Setup which office areas are accessible.
        labs.accessible = d.LabsAccessible;
        comms.accessible = d.CommsAccessible;
        market.accessible = d.MarketAccessible;
    }

    public void BuyLabs() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.LabsAccessible = true;
            labs.accessible = true;
        }
    }

    public void BuyComms() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.CommsAccessible = true;
            comms.accessible = true;
        }
    }

    public void BuyMarket() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (playerCompany.Pay(cost)) {
            data.MarketAccessible = true;
            market.accessible = true;
        }
    }

}
