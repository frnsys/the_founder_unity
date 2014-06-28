using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Item {
    public string name;
    public float cost;
    public float duration;

    public List<StatBuff> productBuffs = new List<StatBuff>();
    public List<StatBuff> workerBuffs = new List<StatBuff>();
    public List<Industry> industries = new List<Industry>();
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Market> markets = new List<Market>();

    public static JSONClass items;

    public Item(string name_, float cost_, List<Industry> industries_,
        List<ProductType> productTypes_, List<Market> markets_
    ) {
        name = name_;
        cost = cost_;
        industries = industries_;
        productTypes = productTypes_;
        markets = markets_;
        LoadStats();
    }

    private void LoadStats() {
        JSONClass item = items[name].AsObject;
        foreach (JSONNode industry in item["Industries"].AsArray) {
            industries.Add(new Industry(industry));
        }
        foreach (JSONNode productType in item["ProductTypes"].AsArray) {
            productTypes.Add(new ProductType(productType));
        }
        foreach (JSONNode market in item["Markets"].AsArray) {
            markets.Add(new Market(market));
        }
    
        // TODO: Is there a DRYer way to do this?
        JSONClass pBuffs = item["ProductBonuses"].AsObject;
        if (pBuffs["Appeal"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Appeal", pBuffs["Appeal"].AsFloat));
        }
        if (pBuffs["Usability"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Usability", pBuffs["Usability"].AsFloat));
        }
        if (pBuffs["Performance"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Performance", pBuffs["Performance"].AsFloat));
        }

        JSONClass wBuffs = item["WorkerBonuses"].AsObject;
        if (wBuffs["Happiness"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Happiness", wBuffs["Happiness"].AsFloat));
        }
        if (pBuffs["Productivity"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Productivity", wBuffs["Productivity"].AsFloat));
        }
        if (pBuffs["Charisma"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Charisma", wBuffs["Charisma"].AsFloat));
        }
        if (pBuffs["Creativity"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Creativity", wBuffs["Creativity"].AsFloat));
        }
        if (pBuffs["Cleverness"].AsFloat > 0) {
            productBuffs.Add(new StatBuff("Cleverness", wBuffs["Cleverness"].AsFloat));
        }  
    }
}


