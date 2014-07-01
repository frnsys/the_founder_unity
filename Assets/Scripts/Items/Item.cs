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
        //JSONClass item = items[name].AsObject;
        //foreach (JSONNode industry in item["Industries"].AsArray) {
            //industries.Add(new Industry(industry));
        //}
        //foreach (JSONNode productType in item["ProductTypes"].AsArray) {
            //productTypes.Add(new ProductType(productType));
        //}
        //foreach (JSONNode market in item["Markets"].AsArray) {
            //markets.Add(new Market(market));
        //}

        //foreach (KeyValuePair<string, JSONNode> pBonuses in item["ProductBonuses"].AsObject) {
            //if (pBonuses.Value.AsFloat > 0) {
                //productBuffs.Add(new StatBuff(pBonuses.Key, pBonuses.Value.AsFloat));
            //}
        //}
        //foreach (KeyValuePair<string, JSONNode> wBonuses in item["WorkerBonuses"].AsObject) {
            //if (wBonuses.Value.AsFloat > 0) {
                //productBuffs.Add(new StatBuff(wBonuses.Key, wBonuses.Value.AsFloat));
            //}
        //}
    }
}


