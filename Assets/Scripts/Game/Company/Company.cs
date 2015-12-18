/*
 * The company is the primary entity of the game.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Smooth.Slinq;

[System.Serializable]
public class Company : HasStats {
    public Stat cash;
    public List<EffectSet> activeEffects;
    public Office.Type office;

    public Company(string name_) {
        name = name_;
    }

    public Company Init() {
        // Default values.
        cash = new Stat("Cash", 100000);
        deathToll = 0;
        debtOwned = 0;
        taxesAvoided = 0;
        lifetimeRevenue = 0;
        annualRevenue = 0;
        annualCosts = 0;
        lastAnnualRevenue = 0;
        lastAnnualCosts = 0;
        baseSizeLimit = 5;
        perks = new List<APerk>();
        office = Office.Type.Apartment;
        recruitments = new List<int> {0};
        promos = new List<int> {0};

        founders = new List<AWorker>();
        _workers = new List<AWorker>();
        _locations = new List<Location>();
        _verticals = new List<Vertical>();
        technologies = new List<Technology>();
        specialProjects = new List<SpecialProject>();
        lobbies = new List<Lobby>();
        markets = new List<MarketManager.Market>();
        marketShare = 0;
        marketSharePercent = 0;

        opinion = new Stat("Outrage", 1);
        opinionEvents = new List<OpinionEvent>();
        publicity = new Stat("Hype", 0);

        activeEffects = new List<EffectSet>();

        companies = new List<MiniCompany>();

        return this;
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    [SerializeField]
    private List<AWorker> _workers;
    public ReadOnlyCollection<AWorker> workers {
        get { return _workers.AsReadOnly(); }
    }
    public IEnumerable<AWorker> allWorkers {
        get { return _workers.Concat(founders.Cast<AWorker>()); }
    }
    public List<AWorker> founders;

    public int baseSizeLimit;
    public int sizeLimit {
        // You can manage 5 employees at your HQ, other locations are managed by one employee.
        // -1 to account for the starting location.
        get { return baseSizeLimit + locations.Count - 1; }
    }
    public int remainingSpace {
        get { return sizeLimit - _workers.Count; }
    }

    static public event System.Action<AWorker, Company> WorkerHired;
    public bool HireWorker(AWorker worker) {
        if (_workers.Count < sizeLimit && Pay(worker.hiringFee)) {
            // Apply existing worker effects.
            foreach (EffectSet es in activeEffects) {
                es.Apply(worker);
            }

            _workers.Add(worker);

            if (WorkerHired != null) {
                WorkerHired(worker, this);
            }

            return true;
        }
        return false;
    }

    static public event System.Action<AWorker, Company> WorkerFired;
    public void FireWorker(AWorker worker) {
        // Remove existing worker effects.
        foreach (EffectSet es in activeEffects) {
            es.Remove(worker);
        }

        worker.salary = 0;
        _workers.Remove(worker);

        if (WorkerFired != null) {
            WorkerFired(worker, this);
        }
    }

    public float productivity {
        get { return AggregateWorkerStat("Productivity"); }
    }
    public float charisma {
        get { return AggregateWorkerStat("Charisma"); }
    }

    public float AggregateWorkerStat(string stat) {
        switch (stat) {
            case "Charisma":
                return allWorkers.Slinq().Select(x => x.charisma).Sum();
            case "Cleverness":
                return allWorkers.Slinq().Select(x => x.cleverness).Sum();
            case "Creativity":
                return allWorkers.Slinq().Select(x => x.creativity).Sum();
            case "Productivity":
                return allWorkers.Slinq().Select(x => x.productivity).Sum();
            case "Happiness":
                return allWorkers.Slinq().Select(x => x.happiness).Sum();
            default:
                return 0;
        }
    }

    // ===============================================
    // Office Management =============================
    // ===============================================

    public bool UpgradeOffice(Office o) {
        if (Pay(o.cost)) {
            office = o.type;
            return true;
        }
        return false;
    }

    // ===============================================
    // Location Management ===========================
    // ===============================================

    [SerializeField]
    private List<Location> _locations;
    public ReadOnlyCollection<Location> locations {
        get { return _locations.AsReadOnly(); }
    }
    public List<MarketManager.Market> markets;
    public float marketShare;
    public float marketSharePercent;
    public int LocationsForMarket(MarketManager.Market m) {
        return locations.Where(l => l.market == m).Count();
    }

    public bool HasLocation(Location loc) {
        return locations.Contains(loc);
    }

    public void SetHQ(Location l) {
        _locations.Add(l);
        markets.Add(l.market);
        l.effects.Apply(this);
        UpdateMarketShare();
    }

    private void UpdateMarketShare() {
        foreach (MarketManager.Market m in MarketManager.Markets) {
            marketShare += LocationsForMarket(m)/MarketManager.marketLocations[m] * MarketManager.SizeForMarket(m);
        }
        marketSharePercent = marketShare/MarketManager.totalMarketSize;
    }

    public bool ExpandToLocation(Location l) {
        if (Pay(l.cost)) {
            _locations.Add(l);

            // The location's market region is now available.
            if (!markets.Contains(l.market))
                markets.Add(l.market);
            UpdateMarketShare();

            // Note this doesn't apply unlock effects...for now assuming locations don't have those.
            l.effects.Apply(this);
            return true;
        }
        return false;
    }


    // ===============================================
    // Vertical Management ===========================
    // ===============================================

    [SerializeField]
    private List<Vertical> _verticals;
    public List<Vertical> verticals {
        get { return _verticals; }
        set { _verticals = value; }
    }

    public bool ExpandToVertical(Vertical v) {
        if (Pay(v.cost)) {
            _verticals.Add(v);
            return true;
        }
        return false;
    }

    // ===============================================
    // Product Management ============================
    // ===============================================

    public Product developingProduct;
    public int launchedProducts;

    static public event System.Action<Product, Company> BeganProduct;
    public Product StartNewProduct(List<ProductType> pts, int design, int marketing, int engineering) {
        Product product = ScriptableObject.CreateInstance<Product>();
        product.Init(pts, design, marketing, engineering, this);
        developingProduct = product;

        if (BeganProduct != null)
            BeganProduct(product, this);

        return product;
    }

    public void HarvestProducts(float elapsedTime) {
        // TODO update this
        //float newRevenue = 0;
        //foreach (Product product in products.Where(p => p.launched)) {
            //newRevenue += product.Revenue(elapsedTime, this);

            //if (product.killsPeople)
                //deathToll += Random.Range(0, 10);

            //if (product.debtsPeople)
                //debtOwned += Random.Range(0, 10);

            //if (product.pollutes)
                //pollution += Random.Range(0, 10);
        //}
        //cash.baseValue += newRevenue;
        //annualRevenue += newRevenue;
        //lifetimeRevenue += newRevenue;
    }

    // TODO remove this
    public void CompletedProduct(Product p) {
        UpdateProductSynergies();

        // Apply relevant effects to the product
        foreach (EffectSet es in activeEffects) {
            es.Apply(p);
        }

        developingProduct = null;
        launchedProducts++;

        // The product's effects are applied by the GameManager.
    }

    static public event System.Action Synergy;
    private void UpdateProductSynergies() {
        // TODO update
        //List<ProductRecipe> activeRecipes = activeProducts.Select(p => p.Recipe).ToList();
        //foreach (Product p in activeProducts) {
            //p.synergy = true;
            //if (p.synergies.Count == 0) {
                //p.synergy = false;
            //} else {
                //for (int i=0; i<p.synergies.Count; i++) {
                    //if (!activeRecipes.Contains(p.synergies[i])) {
                        //p.synergy = false;
                        //break;
                    //}
                //}
            //}
            //if (p.synergy && Synergy != null)
                //Synergy();
        //}
    }

    // ===============================================
    // Other Management ==============================
    // ===============================================

    public List<SpecialProject> specialProjects;
    public bool BuySpecialProject(SpecialProject p) {
        if (Pay(p.cost)) {
            specialProjects.Add(p);
            p.Develop();

            // The product's effects are applied by the GameManager.
            return true;
        }
        return false;
    }


    public List<int> recruitments;
    public void StartRecruitment(Recruitment recruitment) {
        recruitment.Develop();
    }
    public bool BuyRecruitment(Recruitment r) {
        if (Pay(r.cost)) {
            recruitments.Add(r.id);
            return true;
        }
        return false;
    }

    public List<Lobby> lobbies;
    public bool BuyLobby(Lobby l) {
        if (Pay(l.cost)) {
            lobbies.Add(l);
            l.effects.Apply(this);
            return true;
        }
        return false;
    }

    // ===============================================
    // Hype Management ===============================
    // ===============================================

    public void ApplyOpinionEvent(OpinionEvent oe) {
        opinion.ApplyBuff(oe.opinion);
        publicity.baseValue += oe.publicity.value;
        opinionEvents.Add(oe);
    }

    public void StartPromo(Promo promo) {
        promo.Develop();
    }

    public List<int> promos;
    public bool BuyPromo(Promo p) {
        if (Pay(p.cost)) {
            promos.Add(p.id);
            return true;
        }
        return false;
    }


    // ===============================================
    // Financial Management ==========================
    // ===============================================

    public float annualRevenue;
    public float annualCosts;
    public float lastAnnualRevenue;
    public float lastAnnualCosts;
    public float lifetimeRevenue;
    public float taxesAvoided;
    public int deathToll;
    public int debtOwned;
    public int pollution;
    public float annualProfit {
        get { return annualRevenue - annualCosts; }
    }

    static public event System.Action<float, string> Paid;
    public void PayAnnual() {
        float toPay = 0;

        // Skip the first location's rent since it is our HQ and considered free.
        float salaries = workers.Slinq().Select(w => w.salary).Sum() * GameManager.Instance.wageMultiplier;
        float rent = locations.Skip(1).Slinq().Select(l => l.cost).Sum()/1000 * GameManager.Instance.costMultiplier * 12;
        toPay += salaries + rent;

        // Taxes
        float taxes = annualRevenue * GameManager.Instance.taxRate;
        toPay += taxes;

        // The base tax rate is 0.3f.
        float expectedTaxes = annualRevenue * 0.3f;
        taxesAvoided += expectedTaxes - taxes;

        cash.baseValue -= toPay;
        annualCosts = toPay;

        // Also reset annual revenue.
        annualRevenue = 0;

        // TODO move this separately?
        if (Paid != null) {
            Paid(salaries, "for salaries");
            Paid(rent, "for rent");
            Paid(taxes, "for taxes");
        }
    }

    public bool Pay(float cost) {
        if (cash.baseValue - cost >= 0) {
            cash.baseValue -= cost;
            annualCosts += cost;
            return true;
        }
        return false;
    }

    // ===============================================
    // Acquisition Management ========================
    // ===============================================
    public List<MiniCompany> companies;

    public bool BuyCompany(MiniCompany company) {
        if (Pay(company.cost)) {
            // Check if there is an AI company which needs to be disabled.
            if (company.aiCompany != null) {
                AICompany.Find(company.aiCompany).disabled = true;
            }

            companies.Add(company);
            company.effects.Apply(this);
            return true;
        }
        return false;
    }

    public void HarvestCompanies() {
        float newRevenue = 0;
        for (int i=0; i < companies.Count; i++) {
            newRevenue += companies[i].revenue;
        }
        cash.baseValue += newRevenue;
        annualRevenue += newRevenue;
        lifetimeRevenue += newRevenue;
    }

    // ===============================================
    // Perk Management ===============================
    // ===============================================
    public List<APerk> perks;

    static public event System.Action<APerk> PerkBought;
    public bool BuyPerk(APerk perk) {
        if (Pay(perk.cost)) {
            perks.Add(perk);
            perk.upgradeLevel = 0;
            perk.effects.Apply(this);

            if (PerkBought != null)
                PerkBought(perk);
            return true;
        }
        return false;
    }

    public bool UpgradePerk(APerk perk) {
        if (Pay(perk.next.cost)) {
            // First unapply the previous upgrade's effects.
            perk.effects.Remove(this);

            // Upgrade.
            perk.upgradeLevel++;
            perk.effects.Apply(this);

            if (PerkBought != null)
                PerkBought(perk);
            return true;
        }
        return false;
    }

    public APerk OwnedPerk(APerk perk) {
        return perks.Where(p => p.name == perk.name).SingleOrDefault();
    }

    public void RemovePerk(APerk perk) {
        perks.Remove(perk);
        perk.effects.Remove(this);
    }


    // ===============================================
    // Public Opinion ================================
    // ===============================================
    public Stat opinion;
    public Stat publicity;

    [SerializeField]
    private List<OpinionEvent> opinionEvents;
    public ReadOnlyCollection<OpinionEvent> OpinionEvents {
        get { return opinionEvents.AsReadOnly(); }
    }
    public void ForgetOpinionEvents() {
        foreach (OpinionEvent oe in opinionEvents) {
            oe.Forget(GameManager.Instance.forgettingRate);
        }
    }


    // ===============================================
    // Research ======================================
    // ===============================================
    public List<Technology> technologies;
    static public event System.Action<Technology> ResearchCompleted;
    public bool BuyTechnology(Technology t) {
        if (Pay(t.cost)) {
            technologies.Add(t);
            if (ResearchCompleted != null)
                ResearchCompleted(t);
            return true;
        }
        return false;
    }


    // ===============================================
    // Performance Data ==============================
    // ===============================================

    public Dictionary<string, float> CollectAnnualPerformanceData() {
        Dictionary<string, float> results = new Dictionary<string, float>();
        // Compare this annual's performance to last annual's (if available).
        if (lastAnnualRevenue != 0 && lastAnnualCosts != 0) {
            if (lastAnnualRevenue == 0) {
                results["Annual Revenue Delta"] = 0f;
            } else {
                results["Annual Revenue Delta"] = annualRevenue/lastAnnualRevenue;
            }

            if (lastAnnualCosts == 0) {
                results["Annual Costs Delta"] = 0f;
            } else {
                results["Annual Costs Delta"] = annualCosts/lastAnnualCosts;
            }

            float lastAnnualProfit = lastAnnualRevenue - lastAnnualCosts;
            if (lastAnnualProfit == 0) {
                results["Annual Profit Delta"] = 0f;
            } else {
                results["Annual Profit Delta"] = annualProfit/lastAnnualProfit;
            }

        // Otherwise, everything improved by 100%!!!
        } else {
            results["Annual Revenue Delta"] = 1f;
            results["Annual Costs Delta"] = 1f;
            results["Annual Profit Delta"] = 1f;
        }

        results["Annual Revenue"] = annualRevenue;
        results["Annual Costs"] = annualCosts;
        results["Annual Profit"] = annualProfit;

        // Reset values.
        lastAnnualRevenue = annualRevenue;
        lastAnnualCosts = annualCosts;
        annualRevenue = 0;
        annualCosts = 0;

        return results;
    }

    // ===============================================
    // Utility =======================================
    // ===============================================

    public override Stat StatByName(string name) {
        switch (name) {
            case "Cash":
                return cash;
            default:
                return null;
        }
    }
}


