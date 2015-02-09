using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Product : HasStats {
    public enum State {
        DEVELOPMENT,
        LAUNCHED,
        RETIRED
    }

    public string description {
        get {
            return recipe.description != null ? recipe.description : "There's not a lot to say about this product.";
        }
    }

    // A generic name just based on the product types.
    public string genericName {
        get {
            return string.Join(" + ", productTypes.Select(pt => pt.name).ToArray());
        }
    }

    [SerializeField]
    private float _progress = 0;
    public float progress {
        get { return _progress/requiredProgress; }
    }

    public float marketScore = 0;
    public float marketShare = 0;

    public Mesh mesh {
        get {
            // Fallback to first product type's mesh.
            return recipe.mesh != null ? recipe.mesh : productTypes[0].mesh;
        }
    }
    public Texture texture {
        get {
            // Fallback to first product type's texture.
            return recipe.texture != null ? recipe.texture : productTypes[0].texture;
        }
    }

    public float requiredProgress;

    [SerializeField]
    private State _state = State.DEVELOPMENT;
    public State state {
        get { return _state; }
    }

    // A product may be disabled if the company
    // has less infrastructure than necessary to support it.
    // A disabled product generates no revenue and does not continue developing.
    public bool disabled = false;

    // Infrastructure, in points, used by the product.
    public int points {
        get { return productTypes.Sum(p => p.points); }
    }
    public Infrastructure requiredInfrastructure {
        get {
            Infrastructure infras = new Infrastructure();
            foreach (ProductType pt in productTypes) {
                if (pt.requiredInfrastructure != null)
                    infras += pt.requiredInfrastructure;
            }
            return infras;
        }
    }
    public List<Vertical> requiredVerticals {
        get {
            List<Vertical> verts = new List<Vertical>();
            foreach (ProductType pt in productTypes) {
                verts.AddRange(pt.requiredVerticals);
            }
            return verts.Distinct().ToList();
        }
    }
    public EffectSet effects {
        get { return recipe.effects; }
    }

    public bool launched { get { return _state == State.LAUNCHED; } }
    public bool developing { get { return _state == State.DEVELOPMENT; } }
    public bool retired { get { return _state == State.RETIRED; } }

    // All the data about how well
    // this ProductType combination does.
    [SerializeField]
    private ProductRecipe recipe;
    public ProductRecipe Recipe {
        get { return recipe; }
    }

    // The difficulty of a product is the average of its product types' difficulties.
    public float difficulty {
        get { return productTypes.Sum(pt => pt.difficulty)/productTypes.Count; }
    }

    public float timeSinceLaunch = 0;

    // Revenue earned over the product's lifetime.
    public float revenueEarned = 0;

    // Revenue earned during the last cycle.
    public float lastRevenue = 0;

    // Maximum revenue you can make off this product.
    [SerializeField]
    private float peakRevenuePercent;

    [SerializeField]
    private float endFuncAdjustment;

    // How long the product lasts at its peak plateau.
    [SerializeField]
    private float longevity;

    // Revenue model parameters.
    [SerializeField]
    private float start_mu;
    [SerializeField]
    private float start_sd;
    [SerializeField]
    private float end_mu;
    [SerializeField]
    private float end_sd;

    public List<ProductType> productTypes;

    public Stat design;
    public Stat marketing;
    public Stat engineering;

    public void Init(List<ProductType> pts, int design_, int marketing_, int engineering_, Company c) {
        productTypes = pts;

        design =      new Stat("Design",      (float)design_);
        marketing =   new Stat("Marketing",   (float)marketing_);
        engineering = new Stat("Engineering", (float)engineering_);

        requiredProgress = TotalProgressRequired(c);

        recipe = ProductRecipe.LoadFromTypes(pts);

        // Load default if we got nothing.
        if (recipe == null) {
            recipe = ProductRecipe.LoadDefault();
        }

        name = GenerateName(c);
    }

    // Generate a product name.
    private string GenerateName(Company c) {
        if (recipe.names != null) {
            // If the company already has products of this recipe,
            // use "versioning" for the product name.
            IEnumerable<Product> existing = c.products.Where(p => p.Recipe == recipe);
            int version = existing.Count();
            if (version > 0)
                return string.Format("{0} {1}.0", existing.First().name, version + 1);


            // TO DO this can potentially lead to products with duplicate names. Should keep track of which names are used,
            string[] names = recipe.names.Split(new string[] { ", ", "," }, System.StringSplitOptions.None);
            if (names.Length > 0)
                return names[Random.Range(0, names.Length-1)];
        }

        // Fallback to a rather generic name.
        return genericName;
    }


    static public event System.Action<Product, Company> Completed;

    public bool Develop(float newProgress, Company company) {
        if (developing && !disabled) {
            _progress += newProgress;

            if (_progress >= requiredProgress) {
                Launch();

                // Trigger completed event.
                if (Completed != null) {
                    Completed(this, company);
                }
                return true;
            }
        }
        return false;
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

        float A = design.value;
        float U = marketing.value;
        float P = engineering.value;

        // Weights
        float a_w = recipe.design_W;
        float u_w = recipe.marketing_W;
        float p_w = recipe.engineering_W;

        // Ideals
        float a_i = recipe.design_I;
        float u_i = recipe.marketing_I;
        float p_i = recipe.engineering_I;

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

    public float Revenue(float elapsedTime, Company company) {
        timeSinceLaunch += elapsedTime;

        float revenuePercent = 0;
        if (launched && !disabled) {

            // Start
            if (timeSinceLaunch < start_mu) {
                revenuePercent = Gaussian(timeSinceLaunch, start_mu, start_sd);
                //Debug.Log("START FUNC");

            // End
            } else if (timeSinceLaunch > end_mu) {
                // We apply an extra downward weight at the end (0.05f*timeSinceLaunch)
                // to ensure that the end function eventually intersects the x-axis (reaches 0).
                revenuePercent = Gaussian(timeSinceLaunch, end_mu, end_sd) + endFuncAdjustment - (0.05f * timeSinceLaunch);
                //Debug.Log("END FUNC");

            // Plateau
            } else {
                revenuePercent = peakRevenuePercent;
                //Debug.Log("PLATEAU FUNC");
            }

            // Economy's impacts.
            revenuePercent *= GameManager.Instance.economyMultiplier;
            revenuePercent *= GameManager.Instance.spendingMultiplier;

            // Public opinion's impact.
            revenuePercent += company.opinion.value;

            revenuePercent *= marketShare;
        }

        //Debug.Log("REVENUE%:" + revenuePercent);

        // Revenue cannot be negative.
        // Random multiplier for some slight variance.
        float revenue = System.Math.Max(0, revenuePercent * recipe.maxRevenue * Random.Range(0.95f, 1.05f));

        revenueEarned += revenue;
        lastRevenue = revenue;
        return revenue;
    }

    public override Stat StatByName(string name) {
        switch (name) {
            case "Design":
                return design;
            case "Marketing":
                return marketing;
            case "Engineering":
                return engineering;
            default:
                return null;
        }
    }

    // Product death
    public void Shutdown() {
        if (_state == State.DEVELOPMENT) {
            // Give it a basic name; incomplete products aren't christened!
            name = genericName;
        }

        _state = State.RETIRED;
    }


    // Progress required for the nth point.
    public static int baseProgress = 500;
    public float ProgressRequired(string feature, int n, Company c) {
        float reqProgress = Fibonacci(n+2) * baseProgress;
        float aggStat = 0;
        reqProgress *= difficulty;

        switch (feature) {
            case "Design":
                aggStat = c.AggregateWorkerStat("Creativity");
                break;
            case "Engineering":
                aggStat = c.AggregateWorkerStat("Cleverness");
                break;
            case "Marketing":
                aggStat = c.AggregateWorkerStat("Charisma");
                break;
            default:
                break;
        }

        if (aggStat == 0)
            return reqProgress;
        return reqProgress/aggStat;
    }

    public float TotalProgressRequired(Company c) {
        float reqProgress = 0;

        // We only count the base value of these stats, since bonuses to them
        // should not penalize development time.
        reqProgress += ProgressRequired("Design",      (int)design.baseValue, c);
        reqProgress += ProgressRequired("Engineering", (int)engineering.baseValue, c);
        reqProgress += ProgressRequired("Marketing",   (int)marketing.baseValue, c);

        // Required progress can't be 0, so set to 1 if it is.
        return Mathf.Max(1, reqProgress);
    }

    public int EstimatedCompletionTime(Company c) {
        float aggProductivity = c.AggregateWorkerStat("Productivity");
        float reqProgress = TotalProgressRequired(c);

        // Products are developed per development cycle.
        // Int to round down because of Hofstadter's Law: "It always takes longer than you expect, even when you take into account Hofstadter's Law."
        return (int)((reqProgress/aggProductivity) * GameManager.CycleTime);
    }

    public int EstimatedCompletionTime(string feature, int n, Company c) {
        float aggProductivity = c.AggregateWorkerStat("Productivity");
        float reqProgress = ProgressRequired(feature, n, c);
        return (int)((reqProgress/aggProductivity) * GameManager.CycleTime);
    }


    private static int Fibonacci(int n) {
        if (n == 0)
            return 0;
        else if (n == 1)
            return 1;
        else
            return Fibonacci(n-1) + Fibonacci(n-2);
    }
    private static float Gaussian(float x, float mean, float sd) {
        return ( 1 / ( sd * (float)System.Math.Sqrt(2 * (float)System.Math.PI) ) ) * (float)System.Math.Exp( -System.Math.Pow(x - mean, 2) / ( 2 * System.Math.Pow(sd, 2) ) );
    }
    private static float LimitRange(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }
}


