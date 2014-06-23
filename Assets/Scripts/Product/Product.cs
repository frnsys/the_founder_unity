using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum RevenueModel {
    GAUSSIAN,
}

public interface IProduct {
    void Develop(float newProgress, float charisma, float creativity, float cleverness);
    float Revenue(int time);
    void Launch();
    void Shutdown();
}

public class Product : IProduct {
    // Cache the product names and interactions.
    private static JSONClass productNames;
    private static JSONClass productInteractions;

    public string name;

    private float _progress = 0;
    public float progress {
        get { return _progress; }
    }
    private float progressRequired;
    private bool _completed = false;
    public bool completed {
        get { return _completed; }
    }


    private RevenueModel _revenueModel;
    public RevenueModel revenueModel {
        get { return _revenueModel; }
    }

    // Weights
    private float appeal_W;
    private float usability_W;
    private float performance_W;

    // Thresholds
    private float appeal_T;
    private float usability_T;
    private float performance_T;

    public string result;

    public ProductType productType;
    public Industry industry;
    public Market market;


    // Creativity + Charisma
    public Stat appeal = new Stat("Appeal", 0);

    // Cleverness + Charisma
    public Stat usability = new Stat("Usability", 0); // or User Experience?

    // Creativity + Cleverness
    public Stat performance = new Stat("Performance", 0);



    public Product(ProductType pt, Industry i, Market m) {
        name = GenerateName();
        productType = pt;
        industry = i;
        market = m;

        LoadInteraction();
    }


    // Create a random stupid product name.
    private string GenerateName() {
        // Load product name data if necessary.
        if (productNames == null) {
            TextAsset pN = Resources.Load("ProductNames") as TextAsset;
            productNames = JSON.Parse(pN.text).AsObject;
        }

        JSONArray fodder = productNames["fodder"].AsArray;
        JSONArray endings = productNames["endings"].AsArray;

        string beginning = fodder[Random.Range(0, fodder.Count)];
        string middle = fodder[Random.Range(0, fodder.Count)];
        string end = endings[Random.Range(0, endings.Count)];
        beginning = beginning[0].ToString().ToUpper() + beginning.Substring(1);
        return beginning + middle + end;
    }

    // Calculate the effects of the ProductType/Industry/Market interactions.
    private void LoadInteraction() {
        if (productInteractions == null) {
            TextAsset pI = Resources.Load("ProductInteractions") as TextAsset;
            productInteractions = JSON.Parse(pI.text).AsObject;
        }

        // I = Interaction
        JSONClass I = productInteractions[productType.name][industry.name][market.name].AsObject;

        _revenueModel = (RevenueModel)System.Enum.GetValues(typeof(RevenueModel)).GetValue(I["revenue_model"].AsInt);
        appeal_W = I["appeal_weight"].AsFloat;
        usability_W = I["usability_weight"].AsFloat;
        performance_W = I["performance_weight"].AsFloat;
        progressRequired = I["cycles"].AsFloat;

        JSONArray results = I["results"].AsArray;
        result = results[Random.Range(0, results.Count)];
    }

    #region IProduct implementation

    public void Develop(float newProgress, float charisma, float creativity, float cleverness) {
        if (!_completed) {
            float newAppeal = (creativity + charisma)/2;
            float newUsability = (cleverness + charisma)/2;
            float newPerformance = (creativity + cleverness)/2;

            _progress += newProgress;
            appeal.baseValue += newAppeal;
            usability.baseValue += newUsability;
            performance.baseValue += newPerformance;

            if (progress >= progressRequired) {
                _completed = true;
                Launch();
            }
        }
    }

    public void Launch() {
        // ...
        // Modify product-related event probabilities, etc.
    }

    public float Revenue(int time) {
        if (_completed) {
            float A = appeal.value;
            float U = usability.value;
            float P = performance.value;

            // Weights
            float a_w = appeal_W;
            float u_w = usability_W;
            float p_w = performance_W;

            // Thresholds
            float a_t = appeal_T;
            float u_t = usability_T;
            float p_t = performance_T;

            // To be replaced by real values...
            float bonus = 0;
            float economy_w = 1;
            float event_c = 0;

            switch(_revenueModel) {
                case RevenueModel.GAUSSIAN:
                    // This is not the gaussian at all but just temporary, simple polynomial
                    return (((A-a_t)*a_w)*time + ((U-u_t)*u_w)*time + ((P-p_t)*p_w)*time + bonus + event_c)*economy_w;
            }
        }
        return 0;
    }

    // Product death
    public void Shutdown() {
        // ...
        // Modify product-related event probabilities, etc.
    }

    #endregion
}


