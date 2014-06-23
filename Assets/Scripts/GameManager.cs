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
        LoadProductTypes();
        LoadIndustries();
        LoadMarkets();

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

    void LoadProductTypes() {
        TextAsset productTypes = Resources.Load("ProductTypes") as TextAsset;
        foreach(JSONNode name in JSON.Parse(productTypes.text).AsArray) {
            unlockedProductTypes.Add(new ProductType(name));
        }
        Debug.Log("Loaded " + unlockedProductTypes.Count + " product types.");
    }
    void LoadIndustries() {
        TextAsset industries = Resources.Load("Industries") as TextAsset;
        foreach(JSONNode name in JSON.Parse(industries.text).AsArray) {
            unlockedIndustries.Add(new Industry(name));
        }
        Debug.Log("Loaded " + unlockedIndustries.Count + " industries.");
    }
    void LoadMarkets() {
        TextAsset markets = Resources.Load("Markets") as TextAsset;
        foreach(JSONNode name in JSON.Parse(markets.text).AsArray) {
            unlockedMarkets.Add(new Market(name));
        }
        Debug.Log("Loaded " + unlockedMarkets.Count + " markets.");
    }
}


