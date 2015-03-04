/*
 * The company is the primary entity of the game.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Serializable]
public class Company : HasStats {
    public Stat cash;
    public List<EffectSet> activeEffects;
    public string slogan;

    public Office.Type office;

    public Company(string name_) {
        name = name_;
    }

    public Company Init() {
        // Default values.
        cash = new Stat("Cash", 100000);
        research = new Stat("Research", 0);
        researchPoints = 0;
        deathToll = 0;
        debtOwned = 0;
        taxesAvoided = 0;
        lifetimeRevenue = 0;
        lastMonthRevenue = 0;
        quarterRevenue = 0;
        quarterCosts = 0;
        baseSizeLimit = 5;
        perks = new List<Perk>();
        office = Office.Type.Apartment;

        products = new List<Product>();
        founders = new List<Founder>();
        _workers = new List<Worker>();
        _locations = new List<Location>();
        _verticals = new List<Vertical>();
        technologies = new List<Technology>();
        specialProjects = new List<SpecialProject>();
        markets = new List<MarketManager.Market>();

        opinion = new Stat("Opinion", 1);
        opinionEvents = new List<OpinionEvent>();
        publicity = new Stat("Hype", 0);

        activeEffects = new List<EffectSet>();

        companies = new List<MiniCompany>();

        // Keep track for 10 quarters.
        QuarterlyPerfHistory = new PerformanceHistory(10);

        return this;
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    [SerializeField]
    private List<Worker> _workers;
    public ReadOnlyCollection<Worker> workers {
        get { return _workers.AsReadOnly(); }
    }
    public IEnumerable<Worker> allWorkers {
        get { return _workers.Concat(founders.Cast<Worker>()); }
    }
    public List<Founder> founders;

    public int baseSizeLimit;
    public int sizeLimit {
        // You can manage 5 employees at your HQ, other locations are managed by one employee.
        // -1 to account for the starting location.
        get { return baseSizeLimit + locations.Count - 1; }
    }
    public int remainingSpace {
        get { return sizeLimit - _workers.Count; }
    }

    static public event System.Action<Worker, Company> WorkerHired;
    public bool HireWorker(Worker worker) {
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

    static public event System.Action<Worker, Company> WorkerFired;
    public void FireWorker(Worker worker) {
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

    public float AggregateWorkerStat(string stat) {
        switch (stat) {
            case "Charisma":
                return allWorkers.Sum(x => x.charisma.value);
            case "Cleverness":
                return allWorkers.Sum(x => x.cleverness.value);
            case "Creativity":
                return allWorkers.Sum(x => x.creativity.value);
            case "Productivity":
                return allWorkers.Sum(x => x.productivity.value);
            case "Happiness":
                return allWorkers.Sum(x => x.happiness.value);
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
    public int employeesAcrossLocations {
        // TO DO this number is fudged right now. How should it work?
        get { return (locations.Count - 1) * workers.Count + companies.Count * 1000; }
    }
    public List<MarketManager.Market> markets;

    public bool HasLocation(Location loc) {
        return Location.Find(loc, locations) != null;
    }

    public bool ExpandToLocation(Location l) {
        if (Pay(l.cost)) {
            l = l.Clone();
            _locations.Add(l);

            // The location's market region is now available.
            if (!markets.Contains(l.market))
                markets.Add(l.market);

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

    public List<Product> products;
    public Product developingProduct;
    public List<Product> activeProducts {
        get { return products.FindAll(p => p.launched); }
    }
    public List<Product> retiredProducts {
        get { return products.FindAll(p => p.retired); }
    }
    public List<Product> launchedProducts {
        get { return products.FindAll(p => !p.developing); }
    }
    public bool developing {
        get { return developingSpecialProject != null || developingProduct != null; }
    }

    public float totalMarketShare {
        get {
            return activeProducts.Sum(p => p.marketShare)/(activeProducts.Count * 100f);
        }
    }

    static public event System.Action<Product, Company> BeganProduct;
    public void StartNewProduct(List<ProductType> pts, int design, int marketing, int engineering) {
        Product product = ScriptableObject.CreateInstance<Product>();
        product.Init(pts, design, marketing, engineering, this);
        products.Add(product);
        developingProduct = product;

        // Apply relevant effects to the product
        foreach (EffectSet es in activeEffects) {
            es.Apply(developingProduct);
        }

        if (BeganProduct != null)
            BeganProduct(product, this);
    }

    public void AddPointsToDevelopingProduct(string feature, float value) {
        developingProduct.StatByName(feature).baseValue += value;
    }

    public void HarvestProducts(float elapsedTime) {
        float newRevenue = 0;
        foreach (Product product in products.Where(p => p.launched)) {
            newRevenue += product.Revenue(elapsedTime, this);

            if (product.killsPeople)
                deathToll += Random.Range(0, 10);

            if (product.debtsPeople)
                debtOwned += Random.Range(0, 10);
        }
        cash.baseValue += newRevenue;
        lastMonthRevenue += newRevenue;
        quarterRevenue += newRevenue;
        lifetimeRevenue += newRevenue;
    }

    public void DevelopProduct() {
        if (developingProduct != null) {
            bool completed = developingProduct.Develop(1f, this);
            if (completed) {
                UpdateProductSynergies();
                developingProduct = null;

                // The product's effects are applied by the GameManager.
            }
        }
    }

    private void UpdateProductSynergies() {
        List<ProductRecipe> activeRecipes = activeProducts.Select(p => p.Recipe).ToList();
        foreach (Product p in activeProducts) {
            if (p.synergies.Length == 0) {
                p.synergy = false;
            } else {
                for (int i=0; i<p.synergies.Length; i++) {
                    if (!activeRecipes.Contains(p.synergies[i])) {
                        p.synergy = false;
                        break;
                    }
                }
            }
        }
    }

    public void ShutdownProduct(Product product) {
        // Remove relevant effects from the product
        foreach (EffectSet es in activeEffects) {
            es.Remove(product);
        }

        product.Shutdown();
    }

    // Given an item, find the list of currently active products that
    // match at least one of the item's product types.
    public List<Product> FindMatchingProducts(List<ProductType> productTypes) {
        // Items which have no product specifications apply to all products.
        if (productTypes.Count == 0) {
            return products;

        } else {
            return products.FindAll(p =>
                productTypes.Exists(pType => p.productTypes.Contains(pType)));
        }
    }



    // ===============================================
    // Other Management ==============================
    // ===============================================

    public SpecialProject developingSpecialProject;
    public List<SpecialProject> specialProjects;
    public void DevelopSpecialProject() {
        if (developingSpecialProject != null) {
            float progress = 0;
            foreach (Worker worker in allWorkers) {
                progress += worker.productivity.value * Random.Range(0.90f, 1.05f);
                float breakthroughChance = worker.happiness.value/10/workers.Count/100;
                if (Random.value < breakthroughChance) {
                    progress += worker.productivity.value * 0.5f;
                }
            }
            bool completed = developingSpecialProject.Develop(progress);

            if (completed) {
                developingSpecialProject = null;
                specialProjects.Add(developingSpecialProject);
            }
        }
    }
    public bool StartSpecialProject(SpecialProject p) {
        if (Pay(p.cost)) {
            developingSpecialProject = p.Clone();
            return true;
        }
        return false;
    }


    public void StartRecruitment(Recruitment recruitment) {
        developingRecruitment = recruitment.Clone();
    }

    public Recruitment developingRecruitment;
    public void DevelopRecruitment() {
        if (developingRecruitment != null) {
            bool completed = developingRecruitment.Develop();

            if (completed) {
                developingRecruitment = null;
            }
        }
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
        developingPromo = promo.Clone();
    }

    public Promo developingPromo;
    public void DevelopPromo() {
        if (developingPromo != null && opinionCzar != null) {
            bool completed = developingPromo.Develop(opinionCzar.productivity.value, opinionCzar.creativity.value);

            if (completed) {
                UIManager.Instance.LaunchHypeMinigame(developingPromo);
                developingPromo = null;
            }
        }
    }



    // ===============================================
    // Financial Management ==========================
    // ===============================================

    // Keep track of each month's costs.
    public float lastMonthRevenue;
    public float quarterRevenue;
    public float quarterCosts;
    public float lifetimeRevenue;
    public float taxesAvoided;
    public int deathToll;
    public int debtOwned;
    public float quarterProfit {
        get { return quarterRevenue - quarterCosts; }
    }

    static public event System.Action<float, string> Paid;
    public void PayMonthly() {
        float toPay = 0;

        float salaries = workers.Sum(w => w.monthlyPay);
        float rent = locations.Sum(l => l.cost + l.infrastructure.cost);
        toPay += salaries + rent + researchInvestment;

        // Taxes
        float taxes = lastMonthRevenue * GameManager.Instance.taxRate;
        toPay += taxes;

        // The base tax rate is 0.3f.
        float expectedTaxes = lastMonthRevenue * 0.3f;
        taxesAvoided += expectedTaxes - taxes;

        cash.baseValue -= toPay;

        // Add to quarter costs.
        quarterCosts += toPay;

        // Also reset month's revenues.
        lastMonthRevenue = 0;

        if (Paid != null) {
            Paid(salaries, "in salaries");
            Paid(rent, "in rent");
            Paid(researchInvestment, "for research");
            Paid(taxes, "in taxes");
        }
    }

    public bool Pay(float cost) {
        if (cash.baseValue - cost >= 0) {
            cash.baseValue -= cost;
            quarterCosts += cost;
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
        lastMonthRevenue += newRevenue;
        quarterRevenue += newRevenue;
        lifetimeRevenue += newRevenue;
    }

    // ===============================================
    // Perk Management ===============================
    // ===============================================
    public List<Perk> perks;

    static public event System.Action<Perk> PerkBought;
    public bool BuyPerk(Perk perk) {
        perk = perk.Clone();
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

    public bool UpgradePerk(Perk perk) {
        perk = Perk.Find(perk, perks);
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

    public void RemovePerk(Perk perk) {
        perk = Perk.Find(perk, perks);
        perks.Remove(perk);
        perk.effects.Remove(this);
    }


    // ===============================================
    // Infrastructure Management =====================
    // ===============================================

    public Infrastructure infrastructure {
        get {
            IEnumerable<Infrastructure> locationInfra = locations.Select(i => i.infrastructure);
            if (locationInfra.Count() > 0)
                return locationInfra.Aggregate((x,y) => x + y);
            return new Infrastructure();
        }
    }

    // Infrastructure which is available for new products.
    public Infrastructure availableInfrastructure {
        get { return infrastructure - usedInfrastructure; }
    }

    // Infrastructure which is tied up in existing products.
    public Infrastructure usedInfrastructure {
        get {
            Infrastructure usedInfras = new Infrastructure();
            foreach (Product p in products.Where(p => !p.retired)) {
                usedInfras += p.requiredInfrastructure;
            }
            return usedInfras;
        }
    }

    // Total infrastructure capacity.
    public Infrastructure infrastructureCapacity {
        get {
            IEnumerable<Infrastructure> locationCapacities = locations.Select(i => i.capacity);
            if (locationCapacities.Count() > 0)
                return locationCapacities.Aggregate((x,y) => x + y);
            return new Infrastructure();
        }
    }

    // Infrastructure capacity which is unused.
    public Infrastructure availableInfrastructureCapacity {
        get { return infrastructureCapacity - infrastructure; }
    }

    public bool BuyInfrastructure(Infrastructure i, Location loc) {
        loc = Location.Find(loc, locations);
        if (loc.HasCapacityFor(i) && Pay(i.cost)) {
            loc.infrastructure += i;
            UpdateProductStatuses();
            return true;
        }
        return false;
    }

    public void DestroyInfrastructure(Infrastructure i, Location loc) {
        loc = Location.Find(loc, locations);
        loc.infrastructure -= i;
        UpdateProductStatuses();
    }

    // If you don't have enough infrastructure for your
    // current products (in-market & developing), they are put on hold.
    private void UpdateProductStatuses() {
        Infrastructure inf = infrastructure;

        IEnumerable<Product> supportedProducts = products.Where(p => !p.retired);

        // Tally up the total infrastructure required to support all products.
        // Get all products which are currently using infrastructure.
        Infrastructure allInf = new Infrastructure();
        foreach (Product p in supportedProducts) {
            allInf += p.requiredInfrastructure;
        }

        // Figure out which infrastructure types are overloaded.
        List<Infrastructure.Type> overloadedTypes = new List<Infrastructure.Type>();
        foreach (Infrastructure.Type t in Infrastructure.Types) {
            if (allInf[t] > inf[t]) {
                overloadedTypes.Add(t);
            }
        }

        // Disable products which rely on infrastructure which is overloaded.
        // Re-enable products which rely on infrastructure which is no longer overloaded.
        foreach (Product p in supportedProducts) {
            bool set = false;
            foreach (Infrastructure.Type t in overloadedTypes) {
                if (p.requiredInfrastructure[t] > 0) {
                    set = true;
                    break;
                }
            }
            p.disabled = set;
        }
    }

    public bool HasCapacityFor(Infrastructure i) {
        return availableInfrastructureCapacity >= i;
    }


    // ===============================================
    // Public Opinion ================================
    // ===============================================
    [SerializeField]
    private Worker opinionCzar;
    public Worker OpinionCzar {
        get { return opinionCzar; }
        set {
            opinionCzar = value;
            opinion.baseValue = value != null ? opinionCzar.charisma.value : 0;
        }
    }
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
    [SerializeField]
    private Worker researchCzar;
    public Worker ResearchCzar {
        get { return researchCzar; }
        set {
            researchCzar = value;
            research.baseValue = value != null ? researchCzar.cleverness.value : 0;
        }
    }
    public Stat research;
    public int researchPoints;
    public float researchInvestment = 0;
    public List<Technology> technologies;

    public void Research() {
        // TO DO this investment calculation should make more sense.
        researchPoints += (int)(research.value + researchInvestment/1000f);
    }

    static public event System.Action<Technology> ResearchCompleted;
    public bool BuyTechnology(Technology technology) {
        if (researchPoints >= technology.cost) {
            researchPoints -= technology.cost;
            technologies.Add(technology);
            if (ResearchCompleted != null)
                ResearchCompleted(technology);
            return true;
        }
        return false;
    }


    // ===============================================
    // Performance Data ==============================
    // ===============================================

    // Keep track of company performance history as well to try and make decisions.
    [SerializeField]
    protected PerformanceHistory QuarterlyPerfHistory;
    public PerformanceDict lastQuarterPerformance {
        get {
            return QuarterlyPerfHistory.Count > 0 ? QuarterlyPerfHistory.Last() : null;
        }
    }

    // Collect aggregate data for the past quarter.
    public List<PerformanceDict> CollectQuarterlyPerformanceData() {
        PerformanceDict results = new PerformanceDict();
        results["Quarterly Revenue"] = quarterRevenue;
        results["Quarterly Costs"] = quarterCosts;
        results["Quarterly Profit"] = quarterRevenue - quarterCosts;

        QuarterlyPerfHistory.Enqueue(results);

        // Compare this quarter's performance to last quarter's (if available).
        PerformanceDict deltas = new PerformanceDict();
        if (QuarterlyPerfHistory.Count > 1) {
            // Last quarter is the second to last element.
            PerformanceDict lastQuarter = (PerformanceDict)QuarterlyPerfHistory.Skip(QuarterlyPerfHistory.Count - 2).Take(1);

            foreach (string key in results.Keys) {
                deltas[key] = results[key]/lastQuarter[key] - 1f;
            }

        // Otherwise, everything improved by 100%!!!
        } else {
            foreach (string key in results.Keys) {
                deltas[key] = 1f;
            }
        }

        // Reset values.
        quarterRevenue = 0;
        quarterCosts = 0;

        return new List<PerformanceDict> { results, deltas };
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


