using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Product : HasStats {
    public string description {
        get {
            return recipe.description != "" ? recipe.description : "This combination didn't make any sense. This product is incoherent!";
        }
    }

    // A generic name just based on the product types.
    public string genericName {
        get {
            return string.Join(" + ", productTypes.Select(pt => pt.name).ToArray());
        }
    }

    public bool killsPeople;
    public bool debtsPeople;
    public bool pollutes;
    public bool techPenalty;
    public bool synergy;
    public float marketShare;
    public float difficulty;
    public float revenue;

    public Mesh[] meshes {
        get {
            return new Mesh[] {
                productTypes[0].mesh,
                productTypes[1].mesh
            };
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
    public EffectSet effects;

    // All the data about how well
    // this ProductType combination does.
    [SerializeField]
    private ProductRecipe recipe;
    public ProductRecipe Recipe {
        get { return recipe; }
    }

    public List<ProductRecipe> synergies {
        get { return recipe.synergies; }
    }

    // This is identifies what combination of product types
    // the product is. This is meant for quicker comparisons
    // between products to see if they are of the same combo.
    public string comboID;

    [SerializeField]
    private float maxRevenue;

    public List<ProductType> productTypes;

    public Stat design;
    public Stat marketing;
    public Stat engineering;

    public float Create(List<ProductType> pts, float design_, float marketing_, float engineering_, Company c) {
        Init(pts, design_, marketing_, engineering_);

        // Apply relevant effects to the product
        foreach (EffectSet es in c.activeEffects) {
            es.Apply(this);
        }

        // A product recipe can be built without the required techs,
        // but it will operate at a penalty.
        techPenalty = false;
        foreach (Technology t in recipe.requiredTechnologies) {
            if (!c.technologies.Contains(t))
                techPenalty = true;
        }

        name = GenerateName(c);

        return Launch(c);
    }

    public void Init(List<ProductType> pts, float design_, float marketing_, float engineering_) {
        productTypes = pts;
        comboID = string.Join(".", productTypes.OrderBy(pt => pt.name).Select(pt => pt.name).ToArray());

        design =      new Stat("Design",      design_);
        marketing =   new Stat("Marketing",   marketing_);
        engineering = new Stat("Engineering", engineering_);

        recipe = ProductRecipe.LoadFromTypes(pts);

        // Load default if we got nothing.
        if (recipe == null) {
            recipe = ProductRecipe.LoadDefault();
        }

        difficulty = pts[0].difficulty * pts[1].difficulty;

        foreach (Vertical v in requiredVerticals) {
            if (v.name == "Defense") {
                killsPeople = true;
            } else if (v.name == "Finance") {
                debtsPeople = true;
            } else if (v.name == "Hardware") {
                pollutes = true;
            }
        }
    }

    // Generate a product name.
    private string GenerateName(Company c) {
        // TODO
        // If the company already has products of this combo,
        // use "versioning" for the product name.
        //IEnumerable<Product> existing = c.products.Where(p => p.comboID == comboID);
        //int version = existing.Count();
        //if (version > 0)
            //return string.Format("{0} {1}.0", existing.First().name, version + 1);

        //if (recipe.names != null) {
            //// TO DO this can potentially lead to products with duplicate names. Should keep track of which names are used,
            //string[] names = recipe.names.Split(new string[] { ", ", "," }, System.StringSplitOptions.None);
            //if (names.Length > 0)
                //return names[Random.Range(0, names.Length-1)];
        //}

        // Fallback to a rather generic name.
        return genericName;
    }

    static public event System.Action<Product, Company> Completed;

    public float score;
    public float hype;
    public float quality;
    public float Launch(Company company, float hypeBonus=0) {
        // Calculate the revenue model's parameters
        // based on the properties of the product.

        float A = design.value;
        float U = marketing.value;
        float P = engineering.value;

        // Weights
        float a_w = recipe.primaryFeature == ProductRecipe.Feature.Design ? 2f : 1f;
        float u_w = recipe.primaryFeature == ProductRecipe.Feature.Marketing ? 2f : 1f;
        float p_w = recipe.primaryFeature == ProductRecipe.Feature.Engineering ? 2f : 1f;

        // Ideal
        float i = recipe.featureIdeal;

        // Calculate the score, i.e. the percent achieve of the ideal product values.
        // The maximum score is 1.0. We cap each value individually so that
        // they don't "bleed over" into others.
        // We consider engineering and design together to be the "quality" of the product
        float A_ = Mathf.Min((A/i) * a_w, 1f);
        float P_ = Mathf.Min((P/i) * p_w, 1f);
        quality = (A_ + P_)/(a_w + p_w);

        // Marketing is considered separately to be the "hype" around the product
        float U_ = Mathf.Min((U/i) * u_w, 1f);

        float success_chance = (A_ + P_ + U_)/(a_w + p_w + u_w);
        if (Random.value > success_chance) {
            // Failure
            return 0;
        }

        // Negative hype bonus causes a fractional effect to hype
        if (hypeBonus < 0) {
            hypeBonus = 1/(-1 * hypeBonus + 1);
        }
        hype = U_/u_w * hypeBonus;

        marketShare = company.marketSharePercent * quality;

        if (techPenalty)
            marketShare *= 0.1f;

        // Hype matters a lot
        marketShare *= 1 + hype;

        // Public opinion's impact.
        marketShare *= 1 + company.opinion/100f;

        marketShare = Mathf.Min(marketShare, 1f);

        // Maxmimum lifetime revenue of the product.
        // * 10 to make the game a bit easier.
        maxRevenue = recipe.maxRevenue * marketShare * 10;

        // Effect modifications.
        effects = recipe.effects.Clone();
        effects.ApplyMultiplier(quality);

        // Score is weighted towards hype
        score = (quality + (2*hype))/3;

        Debug.Log(string.Format("Quality {0}", quality));
        Debug.Log(string.Format("Hype {0}", hype));
        Debug.Log(string.Format("Score {0}", score));
        Debug.Log(string.Format("Design Value {0}", A));
        Debug.Log(string.Format("Marketing Value {0}", U));
        Debug.Log(string.Format("Engineering Value {0}", P));
        Debug.Log(string.Format("Max Revenue {0}", maxRevenue));



        // Trigger completed event.
        if (Completed != null) {
            Completed(this, company);
        }
        return Revenue(score);
    }

    public float Revenue(float score) {
        float revenue = 0;
        revenue = score * maxRevenue * Random.Range(0.95f, 1.05f);

        // Economy's impact.
        revenue *= GameManager.Instance.economyMultiplier;

        // Consumer spending impact.
        revenue *= GameManager.Instance.spendingMultiplier;

        if (synergy)
            revenue *= 1.5f;

        return Mathf.Max(revenue, 0);
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
}


