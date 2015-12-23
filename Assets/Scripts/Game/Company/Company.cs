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

    public struct StatusReport {
        public float rent;
        public float salaries;
        public float taxes;
        public float costs;
        public float costsDelta;
        public float otherCosts;
        public float profit;
        public float profitDelta;
        public float revenue;
        public float revenueDelta;

        public StatusReport(float annualRevenue, float lastAnnualRevenue, float annualCosts, float lastAnnualCosts, float rentCost, float salaryCost, float taxesCost) {
            revenue = annualRevenue;
            profit = annualRevenue - annualCosts;
            costs = annualCosts;
            taxes = taxesCost;
            salaries = salaryCost;
            rent = rentCost;
            otherCosts = annualCosts - taxes - salaries - rent;

            // Compare this annual's performance to last annual's (if available).
            if (lastAnnualRevenue != 0 && lastAnnualCosts != 0) {
                if (lastAnnualRevenue == 0) {
                    revenueDelta = 0f;
                } else {
                    revenueDelta = revenue/lastAnnualRevenue;
                }

                if (lastAnnualCosts == 0) {
                    costsDelta = 0f;
                } else {
                    costsDelta = costs/lastAnnualCosts;
                }

                float lastAnnualProfit = lastAnnualRevenue - lastAnnualCosts;
                if (lastAnnualProfit == 0) {
                    profitDelta = 0f;
                } else {
                    profitDelta = profit/lastAnnualProfit;
                }

            // Otherwise, everything improved by 100%!!!
            } else {
                revenueDelta = 1f;
                costsDelta = 1f;
                profitDelta = 1f;
            }
        }
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

        hype = 0;

        activeEffects = new List<EffectSet>();
        companies = new List<MiniCompany>();
        discoveredProducts = new List<ProductRecipe>();
        productTypes = new List<ProductType>();

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
    public float happiness {
        get { return AggregateWorkerStat("Happiness"); }
    }
    public float creativity {
        get { return AggregateWorkerStat("Creativity"); }
    }
    public float cleverness {
        get { return AggregateWorkerStat("Cleverness"); }
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

    public void UpdateMarketShare() {
        float totalMarketSize = MarketManager.totalMarketSize;

        if (!GameManager.Instance.extraTerra)
            totalMarketSize -= MarketManager.SizeForMarket(MarketManager.Market.ExtraTerra);
        if (!GameManager.Instance.alien)
            totalMarketSize -= MarketManager.SizeForMarket(MarketManager.Market.Alien);

        foreach (MarketManager.Market m in MarketManager.Markets) {
            marketShare += LocationsForMarket(m)/MarketManager.marketLocations[m] * MarketManager.SizeForMarket(m);
        }
        marketSharePercent = marketShare/totalMarketSize;
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

    public int launchedProducts;
    public List<ProductRecipe> discoveredProducts;
    public List<ProductType> productTypes;
    static public event System.Action<Company, Product> DiscoveredProduct;
    public Product LaunchProduct(List<ProductType> pts, float multiplier) {
        Product product = ScriptableObject.CreateInstance<Product>();
        float revenue = product.Create(pts, creativity, charisma, cleverness, this);
        if (revenue == 0) {
            // Failure
            return null;
        }

        revenue *= multiplier;

        if (product.killsPeople)
            deathToll += Random.Range(0, 10);

        if (product.debtsPeople)
            debtOwned += Random.Range(0, 10);

        if (product.pollutes)
            pollution += Random.Range(0, 10);

        cash.baseValue += revenue;
        annualRevenue += revenue;
        lifetimeRevenue += revenue;
        product.revenue = revenue;

        if (!discoveredProducts.Contains(product.Recipe)) {
            discoveredProducts.Add(product.Recipe);
            DiscoveredProduct(this, product);
        }

        launchedProducts++;
        return product;
    }

    public bool BuyProductType(ProductType pt) {
        if (Pay(pt.cost)) {
            productTypes.Add(pt);
            return true;
        }
        return false;
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
    public float salaries {
        get { return workers.Slinq().Select(w => w.salary).Sum() * GameManager.Instance.wageMultiplier; }
    }
    public float rent {
        // Skip the first location's rent since it is our HQ and considered free.
        get { return locations.Skip(1).Slinq().Select(l => l.cost).Sum()/1000 * GameManager.Instance.costMultiplier * 12; }
    }
    public float taxes {
        get { return annualRevenue * GameManager.Instance.taxRate; }
    }
    public float toPay {
        get { return salaries + rent + taxes; }
    }

    static public event System.Action<float, string> Paid;
    public void PayAnnual() {
        // The base tax rate is 0.3f.
        float expectedTaxes = annualRevenue * 0.3f;
        taxesAvoided += expectedTaxes - taxes;

        cash.baseValue -= toPay;
        annualCosts = toPay;

        // Also reset annual revenue.
        annualRevenue = 0;
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
    public int hype;
    public float opinion;

    public void Forget() {
        float rate = GameManager.Instance.forgettingRate;
        if (opinion > 0) {
            opinion -= rate;

            // If overshot, just set to 0.
            if (opinion < 0)
                opinion = 0;
        } else if (opinion < 0) {
            opinion += rate;

            // If overshot, just set to 0.
            if (opinion > 0)
                opinion = 0;
        }

        hype -= (int)GameManager.Instance.forgettingRate;
        hype = hype < 0 ? 0 : hype;
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

    public StatusReport CollectAnnualPerformanceData() {
        StatusReport report = new StatusReport(annualRevenue, lastAnnualRevenue, annualCosts, lastAnnualCosts, rent, salaries, taxes);

        // Reset values.
        lastAnnualRevenue = annualRevenue;
        lastAnnualCosts = annualCosts;
        annualRevenue = 0;
        annualCosts = 0;

        return report;
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


