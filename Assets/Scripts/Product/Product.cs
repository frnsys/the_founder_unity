using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public interface IProduct {
    void Develop(float newProgress, float charisma, float creativity, float cleverness);
    float Revenue(float time);
    void Launch();
    void Shutdown();
}

public class Product : HasStats, IProduct {
    public enum State {
        DEVELOPMENT,
        LAUNCHED,
        RETIRED
    }

    // Cache the product names and interactions.
    private static JSONClass productNames;

    public string name;

    private float _progress = 0;
    public float progress {
        get { return _progress; }
    }
    private State _state = State.DEVELOPMENT;
    public State state {
        get { return _state; }
    }

    // All the data about how well
    // this ProductType/Industry/Market
    // combination does.
    private ProductRecipe recipe;

    // Maximum revenue you can make off this product.
    private float peakRevenuePercent;
    private float endFuncAdjustment;

    // How long the product lasts at its peak plateau.
    private float longevity;

    // Revenue model parameters.
    private float start_mu;
    private float start_sd;
    private float end_mu;
    private float end_sd;

    public ProductType productType;
    public Industry industry;
    public Market market;

    // Creativity + Charisma
    public Stat appeal;

    // Cleverness + Charisma
    public Stat usability; // or User Experience?

    // Creativity + Cleverness
    public Stat performance;

    public Product(ProductType pt, Industry i, Market m) {
        name = GenerateName();
        productType = pt;
        industry = i;
        market = m;

        appeal = new Stat("Appeal", 0);
        usability = new Stat("Usability", 0);
        performance = new Stat("Performance", 0);

        recipe = ProductRecipe.Load(pt, i, m);

        // Load default if we got nothing.
        if (recipe == null) {
            recipe = ProductRecipe.Load();
        }
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

    #region IProduct implementation

    public void Develop(float newProgress, float charisma, float creativity, float cleverness) {
        if (state == State.DEVELOPMENT) {
            float newAppeal = (creativity + charisma)/2;
            float newUsability = (cleverness + charisma)/2;
            float newPerformance = (creativity + cleverness)/2;

            _progress += newProgress;
            appeal.baseValue += newAppeal;
            usability.baseValue += newUsability;
            performance.baseValue += newPerformance;

            if (progress >= recipe.progressRequired) {
                Launch();
            }
        }
    }

    public void Launch() {
        // Calculate the revenue model's parameters
        // based on the properties of the product.

        // We're using a piecewise function consisting of
        // two normal distributions and one constant.
        // ------------------------------------------
        // Product has three life stages:
        // 1. Start     (normal)
        // 2. Plateau   (constant)
        // 3. End       (normal)
        // |
        // |          2
        // | 1   ____________  3
        // |    /            \
        // |   /              \
        // |__/________________\___
        //
        // The starting stage can either be slow or exponential growth.
        // The ending stage can either be slow or exponential decline.
        //
        // Parameters:
        //    mu = mean, mu, µ.
        //      Positions the peak.
        //    sd = standard deviation, sigma, σ.
        //      Controls breadth. Lower sigmas are steeper curves.
        // Note: Math.Exp(x) = e^x

        // Calculate where to position the graph:
        // --------------------------------------
        // We want the graph to have the property of f(0) = 0 so that it starts
        // at the beginning of the curve when t = 0.
        // Basically we can calculate t where f(t) = 0 and then use the mean to
        // shift the graph's position by -t.
        // 99.7% of the normal distribution's space  is within 3 standard deviations.
        // Since there is not really a position where f(t) = 0, we can use that
        // property as an approximation for it, so:
        // f(t) ≈ 0 for t = mu - 3*sd
        // Then we can calculate the starting mean with mu - (mu - 3*sd),
        // i.e. 3*sd.

        // Another useful property is that max(f(t)) == f(mu),
        // that is, f(t) is at its peak when t = mu.

        float A = appeal.value;
        float U = usability.value;
        float P = performance.value;

        // Weights
        float a_w = recipe.appeal_W;
        float u_w = recipe.usability_W;
        float p_w = recipe.performance_W;

        // Ideals
        float a_i = recipe.appeal_I;
        float u_i = recipe.usability_I;
        float p_i = recipe.performance_I;

        // Adjusted values, min 0 (no negatives).
        float A_ = (A/a_i) * a_w;
        float U_ = (U/u_i) * u_w;
        float P_ = (P/p_i) * p_w;
        float combo = A_ + U_ + P_;

        // Revenue model params:

        // Lower is better (more explosive growth).
        start_sd = LimitRange(1/combo, 0.25f, 3.5f);

        // Higher is better (slower decline).
        end_sd = LimitRange(combo, 0.25f, 3.5f);

        // Time where the plateau begins, see comments above for rationale.
        start_mu = 3 * start_sd;

        // How long the plateau lasts.
        // TO DO tweak this to something that makes more sense.
        longevity = combo/recipe.maxLongevity;

        // Time where the plateau ends
        end_mu = start_mu + longevity;

        // Calculate the peak revenue percentage for the plateau.
        peakRevenuePercent = Gaussian(start_mu, start_mu, start_sd);

        // Calculate the constant required to vertically shift the
        // end function so that it's peak intersects with the starting peak.
        // We apply an extra downward weight at the end (0.05f*end_mu)
        // to ensure that the end function eventually intersects the x-axis (reaches 0).
        float endPeak = Gaussian(end_mu, end_mu, end_sd) - (0.05f * end_mu);
        endFuncAdjustment = peakRevenuePercent - endPeak;

        //Debug.Log("START_SD:" + start_sd);
        //Debug.Log("START_MU:" + start_mu);
        //Debug.Log("END_SD:" + end_sd);
        //Debug.Log("END_MU:" + end_mu);
        //Debug.Log("PEAK REV:" + peakRevenuePercent);
        //Debug.Log("ADJ CONST:" + endFuncAdjustment);

        _state = State.LAUNCHED;
    }

    public float Revenue(float time) {
        //time /= 3600; // adjusting time scale

        float revenuePercent = 0;
        if (state == State.LAUNCHED) {

            // To be replaced by real values...
            float bonus = 0;
            float economy_w = 1;
            float event_c = 0;

            // Start
            if (time < start_mu) {
                revenuePercent = Gaussian(time, start_mu, start_sd);
                //Debug.Log("START FUNC");

            // End
            } else if (time > end_mu) {
                // We apply an extra downward weight at the end (0.05f*time)
                // to ensure that the end function eventually intersects the x-axis (reaches 0).
                revenuePercent = Gaussian(time, end_mu, end_sd) + endFuncAdjustment - (0.05f * time);
                //Debug.Log("END FUNC");

            // Plateau
            } else {
                revenuePercent = peakRevenuePercent;
                //Debug.Log("PLATEAU FUNC");
            }

            // Bonus, event, and economy's impacts.
            revenuePercent += bonus + event_c;
            revenuePercent *= economy_w;
        }

        //Debug.Log("REVENUE%:" + revenuePercent);

        // Revenue cannot be negative.
        // Random multiplier for some slight variance.
        return System.Math.Max(0, revenuePercent * recipe.maxRevenue * Random.Range(0.95f, 1.05f));
    }
    private float Gaussian(float x, float mean, float sd) {
        return ( 1 / ( sd * (float)System.Math.Sqrt(2 * (float)System.Math.PI) ) ) * (float)System.Math.Exp( -System.Math.Pow(x - mean, 2) / ( 2 * System.Math.Pow(sd, 2) ) );
    }
    private float LimitRange(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }

    public void ApplyItem(Item item) {
        ApplyBuffs(item.productBuffs);
    }
    public void RemoveItem(Item item) {
        RemoveBuffs(item.productBuffs);
    }

    public override Stat StatByName(string name) {
        switch (name) {
            case "Appeal":
                return appeal;
            case "Usability":
                return usability;
            case "Performance":
                return performance;
            default:
                return null;
        }
    }

    // Product death
    public void Shutdown() {
        // ...
        // Modify product-related event probabilities, etc.
        _state = State.RETIRED;
    }

    #endregion
}


