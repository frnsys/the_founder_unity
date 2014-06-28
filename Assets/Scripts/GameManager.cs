using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameManager : MonoBehaviour {
    private Company playerCompany;
    private List<ProductType> unlockedProductTypes = new List<ProductType>();
    private List<Industry> unlockedIndustries = new List<Industry>();
    private List<Market> unlockedMarkets = new List<Market>();

    void Start() {
        LoadResources();
        //StartCoroutine(PayYourDebts());
    }

    void Update() {
    }

    IEnumerator PayYourDebts() {
        while(true) {
            playerCompany.Pay();
            yield return new WaitForSeconds(60);
        }
    }

    public void LoadResources() {
        LoadProductTypes();
        LoadIndustries();
        LoadMarkets();
        LoadItems();
    }

    void LoadProductTypes() {
        TextAsset productTypes = Resources.Load("ProductTypes") as TextAsset;
        foreach(JSONNode name in JSON.Parse(productTypes.text).AsArray) {
            unlockedProductTypes.Add(new ProductType(name));
        }
    }
    void LoadIndustries() {
        TextAsset industries = Resources.Load("Industries") as TextAsset;
        foreach(JSONNode name in JSON.Parse(industries.text).AsArray) {
            unlockedIndustries.Add(new Industry(name));
        }
    }
    void LoadMarkets() {
        TextAsset markets = Resources.Load("Markets") as TextAsset;
        foreach(JSONNode name in JSON.Parse(markets.text).AsArray) {
            unlockedMarkets.Add(new Market(name));
        }
    }
    void LoadItems() {
        TextAsset itemText = Resources.Load("Items") as TextAsset;
        Item.items = JSON.Parse(itemText.text).AsObject;
    }
}


