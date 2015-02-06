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

    [SerializeField]
    private Worker researchCzar;
    public Worker ResearchCzar {
        get { return researchCzar; }
        set {
            researchCzar = value;

            if (value != null) {
                research.baseValue = researchCzar.cleverness.value;
            } else {
                research.baseValue = 0;
            }
        }
    }
    public Stat research;
    public float researchInvestment = 1000;

    [SerializeField]
    private Worker opinionCzar;
    public Worker OpinionCzar {
        get { return opinionCzar; }
        set {
            opinionCzar = value;

            if (value != null) {
                opinion.baseValue = opinionCzar.charisma.value;
            } else {
                opinion.baseValue = 0;
            }
        }
    }
    public Stat opinion;
    public Stat publicity;
    public float forgettingRate;

    [SerializeField]
    private List<OpinionEvent> opinionEvents;
    public ReadOnlyCollection<OpinionEvent> OpinionEvents {
        get { return opinionEvents.AsReadOnly(); }
    }
    public void ForgetOpinionEvents() {
        foreach (OpinionEvent oe in opinionEvents) {
            oe.Forget(forgettingRate);
        }
    }

    public List<EffectSet> activeEffects;

    public List<MarketManager.Market> markets;

    public Company(string name_) {
        name = name_;
    }

    public List<Technology> technologies;

    public Company Init() {
        // Default values.
        cash = new Stat("Cash", 100000);
        research = new Stat("Research", 1);
        lastMonthCosts = 0;
        lastMonthRevenue = 0;
        quarterRevenue = 0;
        quarterCosts = 0;
        baseSizeLimit = 5;
        perks = new List<Perk>();

        products = new List<Product>();
        founders = new List<Founder>();
        _workers = new List<Worker>();
        _locations = new List<Location>();
        _verticals = new List<Vertical>() {
            Vertical.Load("Information")
        };
        technologies = new List<Technology>();
        markets = new List<MarketManager.Market>();

        forgettingRate = 1;
        opinion = new Stat("Opinion", 1);
        opinionEvents = new List<OpinionEvent>();
        publicity = new Stat("Publicity", 0);

        activeEffects = new List<EffectSet>();

        companies = new List<MiniCompany>();

        // Keep track for a quarter.
        PerfHistory = new PerformanceHistory(3);
        ProductPerfHistory = new PerformanceHistory(3);
        WorkerPerfHistory = new PerformanceHistory(3);

        // Keep track for 10 quarters.
        QuarterlyPerfHistory = new PerformanceHistory(10);

        return this;
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    public int baseSizeLimit;
    public int sizeLimit {
        // You can manage 5 employees at your HQ, other locations are managed by one employee.
        // -1 to account for the starting location.
        get { return baseSizeLimit + locations.Count - 1; }
    }
    public List<Founder> founders;

    [SerializeField]
    private List<Worker> _workers;

    public ReadOnlyCollection<Worker> workers {
        get { return _workers.AsReadOnly(); }
    }
    public int remainingSpace {
        get { return sizeLimit - _workers.Count; }
    }

    // This does not include czars!
    public IEnumerable<Worker> allWorkers {
        get { return _workers.Concat(founders.Cast<Worker>()).Where(w => w != researchCzar && w != opinionCzar); }
    }

    static public event System.Action<Worker, Company> WorkerHired;
    static public event System.Action<Worker, Company> WorkerFired;
    public bool HireWorker(Worker worker) {
        if (_workers.Count < sizeLimit && Pay(worker.hiringFee)) {
            // Apply existing worker effects.
            foreach (EffectSet es in activeEffects) {
                es.Apply(worker);
            }

            _workers.Add(worker);

            // Update the progress required for developing products.
            foreach (Product p in developingProducts) {
                p.requiredProgress = p.TotalProgressRequired(this);
            }

            if (WorkerHired != null) {
                WorkerHired(worker, this);
            }

            return true;
        }
        return false;
    }
    public void FireWorker(Worker worker) {
        // Remove existing worker effects.
        foreach (EffectSet es in activeEffects) {
            es.Remove(worker);
        }

        worker.salary = 0;

        _workers.Remove(worker);

        // Update the progress required for developing products.
        foreach (Product p in developingProducts) {
            p.requiredProgress = p.TotalProgressRequired(this);
        }

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
    // Location Management ===========================
    // ===============================================

    [SerializeField]
    private List<Location> _locations;
    public ReadOnlyCollection<Location> locations {
        get { return _locations.AsReadOnly(); }
    }

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
    public List<Product> activeProducts {
        get {
            return products.FindAll(p => p.state == Product.State.LAUNCHED);
        }
    }
    public List<Product> developingProducts {
        get {
            return products.FindAll(p => p.state == Product.State.DEVELOPMENT);
        }
    }
    public List<Product> retiredProducts {
        get {
            return products.FindAll(p => p.state == Product.State.RETIRED);
        }
    }
    public List<Product> launchedProducts {
        get {
            return products.FindAll(p => p.state != Product.State.DEVELOPMENT);
        }
    }

    // Products grouped by state.
    public List<Product> sortedProducts {
        get {
            return developingProducts.Concat(activeProducts).Concat(retiredProducts).ToList();
        }
    }

    public bool developing {
        get {
            return developingProducts.Count > 0;
        }
    }

    public void StartNewProduct(List<ProductType> pts, int design, int marketing, int engineering) {
        Product product = ScriptableObject.CreateInstance<Product>();
        product.Init(pts, design, marketing, engineering, this);
        products.Add(product);
    }

    public void DevelopProducts() {
        List<Product> inDevelopment = products.FindAll(p => p.state == Product.State.DEVELOPMENT);
        foreach (Product product in inDevelopment) {
            DevelopProduct(product);
        }
    }

    public Promo developingPromo;
    public void DevelopPromo() {
        if (developingPromo != null && opinionCzar != null) {
            OpinionEvent result = developingPromo.Develop(opinionCzar.productivity.value, opinionCzar.creativity.value);

            if (result != null) {
                ApplyOpinionEvent(result);
                developingPromo = null;
            }
        }
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

    public void ApplyOpinionEvent(OpinionEvent oe) {
        opinion.ApplyBuff(oe.opinion);
        publicity.ApplyBuff(oe.publicity);
        opinionEvents.Add(oe);
    }

    public void StartPromo(Promo promo) {
        developingPromo = promo.Clone();
    }

    public void StartRecruitment(Recruitment recruitment) {
        developingRecruitment = recruitment.Clone();
    }

    public void HarvestProducts(float elapsedTime) {
        List<Product> launched = products.FindAll(p => p.state == Product.State.LAUNCHED);

        float newRevenue = 0;
        foreach (Product product in launched) {
            newRevenue += product.Revenue(elapsedTime, this);
        }
        cash.baseValue += newRevenue;
        lastMonthRevenue += newRevenue;
        quarterRevenue += newRevenue;
        lifetimeRevenue += newRevenue;
    }

    public void DevelopProduct(Product product) {
        float progress = 0;

        foreach (Worker worker in allWorkers) {
            // A bit of randomness to make things more interesting.
            progress += worker.productivity.value * Random.Range(0.90f, 1.05f);

            // Workers have a chance of making a "breakthrough" depending on their
            // happiness. Each point of happiness = 1/10% more of a chance.
            // The chance is weighted by the number of workers at the company as well,
            // so you can't mass-hire to get greater cumulative probabilities of breakthroughs.
            // TO DO this may need to be tweaked.
            float breakthroughChance = worker.happiness.value/10/workers.Count/100;

            // If the breakthrough happens,
            // the product gets a boost for the feature associated with
            // the worker's best stat, based on the value of that stat.
            if (Random.value < breakthroughChance) {
                Stat bestStat = worker.bestStat;
                switch (bestStat.name) {
                    case "Charisma":
                        product.marketing.baseValue += bestStat.value/100;
                        break;
                    case "Cleverness":
                        product.engineering.baseValue += bestStat.value/100;
                        break;
                    case "Creativity":
                        product.design.baseValue += bestStat.value/100;
                        break;
                    default:
                        break;
                }
            }
        }

        bool completed = product.Develop(progress, this);
        if (completed) {
            product.released = true;

            // Apply relevant effects to the product
            foreach (EffectSet es in activeEffects) {
                es.Apply(product);
            }

            // The product's effects are applied by the GameManager.
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
    // Financial Management ==========================
    // ===============================================

    // Keep track of each month's costs.
    public float lastMonthCosts;
    public float lastMonthRevenue;
    public float quarterRevenue;
    public float quarterCosts;
    public float lifetimeRevenue;
    public void PayMonthly() {
        float toPay = 0;
        float salaries = 0;
        float rent = 0;

        foreach (Worker worker in workers) {
            salaries += worker.monthlyPay;
        }
        toPay += salaries;

        foreach (Location loc in locations) {
            rent += loc.cost;

            // We have to add up location infrastructure cost in this way,
            // so we incorporate the cost of the infrastructure for the location.
            rent += loc.infrastructure.cost;
        }
        toPay += rent;

        toPay += researchInvestment;

        // Taxes
        float taxes = lastMonthRevenue * 0.12f;
        toPay += taxes;

        cash.baseValue -= toPay;

        // Reset month's costs with this month's costs.
        lastMonthCosts = toPay;
        quarterCosts += toPay;

        // Also reset month's revenues.
        lastMonthRevenue = 0;

        UIManager.Instance.SendPing(string.Format("Paid {0:C0} in salaries.", salaries), Color.red);
        UIManager.Instance.SendPing(string.Format("Paid {0:C0} in rent.", rent), Color.red);
        UIManager.Instance.SendPing(string.Format("Paid {0:C0} for research.", researchInvestment), Color.red);
        UIManager.Instance.SendPing(string.Format("Paid {0:C0} in taxes.", taxes), Color.red);
    }

    public bool Pay(float cost) {
        if (cash.baseValue - cost >= 0) {
            cash.baseValue -= cost;
            lastMonthCosts += cost;
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

    public bool BuyPerk(Perk perk) {
        perk = perk.Clone();
        if (Pay(perk.cost)) {
            perks.Add(perk);
            perk.upgradeLevel = 0;
            perk.effects.Apply(this);
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

    [SerializeField]
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
        get {
            return infrastructure - usedInfrastructure;
        }
    }

    // Infrastructure which is tied up in existing products.
    public Infrastructure usedInfrastructure {
        get {
            Infrastructure usedInfras = new Infrastructure();
            foreach (Product p in products) {
                if (p.state != Product.State.RETIRED) {
                    usedInfras += p.requiredInfrastructure;
                }
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
        get {
            return infrastructureCapacity - infrastructure;
        }
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

    private void UpdateProductStatuses() {
        Infrastructure inf = infrastructure;

        // Get all products which are currently using infrastructure.
        List<Product> supportedProducts = products.FindAll(p => p.state != Product.State.RETIRED);

        // Tally up the total infrastructure required to support all products.
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
    // Performance Data ==============================
    // ===============================================

    // The Company must surveil its assets to track performance.
    public void CollectPerformanceData() {
        Debug.Log(name + " is collecting performance data...");

        PerfHistory.Enqueue(SamplePerformance());
        ProductPerfHistory.Enqueue(ProductAverages());
        WorkerPerfHistory.Enqueue(WorkerAverages());

        Debug.Log(PerfHistory);
        Debug.Log(ProductPerfHistory);
        Debug.Log(WorkerPerfHistory);
    }

    // Keep track of company performance history as well to try and make decisions.
    [SerializeField]
    protected PerformanceHistory PerfHistory;
    [SerializeField]
    protected PerformanceHistory ProductPerfHistory;
    [SerializeField]
    protected PerformanceHistory WorkerPerfHistory;
    [SerializeField]
    protected PerformanceHistory QuarterlyPerfHistory;
    protected List<Product> ProductsReleased {
        get {
            return products.Where(p => p.released).ToList();
        }
    }
    public PerformanceDict lastQuarterPerformance {
        get {
            int count = QuarterlyPerfHistory.Count;
            if (count > 0) {
                return QuarterlyPerfHistory.Last();
            }
            return null;
        }
    }

    // These data are sampled every month.
    private PerformanceDict SamplePerformance() {
        return new PerformanceDict {
            {"Month Revenue", lastMonthRevenue},
            {"Month Costs", lastMonthCosts}
        };
    }

    // Collect aggregate data for the past quarter.
    public List<PerformanceDict> CollectQuarterlyPerformanceData() {
        PerformanceDict results = new PerformanceDict();
        results["Quarterly Revenue"] = quarterRevenue;
        results["Quarterly Costs"] = quarterCosts;

        float avgPROI = 0;
        foreach (Product p in ProductsReleased) {
            // TO DO this could be more detailed.
            avgPROI += p.revenueEarned/p.points;
        }
        results["Product ROI"] = avgPROI/ProductsReleased.Count;
        QuarterlyPerfHistory.Enqueue(results);

        // Reset the products released for the new quarter.
        ProductsReleased.Clear();

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

    private PerformanceDict ProductAverages() {
        float avgROI = 0;

        if (activeProducts.Count > 0) {
            foreach (Product p in activeProducts) {
                avgROI += ProductROI(p);
            }
            avgROI /= activeProducts.Count;
        }

        return new PerformanceDict {
            {"Average ROI", avgROI}
        };
    }

    // Calculate the return on investment for a product,
    // normalized for time.
    protected float ProductROI(Product p) {
        // TO DO tweak this
        // this should maybe also take into account cash invested
        // (e.g. rent, salaries, etc)
        return (p.revenueEarned/p.timeSinceLaunch)/p.points;
    }

    // Aggregate averages of certain worker stats.
    private PerformanceDict WorkerAverages() {
        PerformanceDict results = new PerformanceDict {
            {"Happiness", 0f},
            {"Productivity", 0f}
        };
        List<string> statNames = results.Keys.ToList();

        if (workers.Count > 0) {
            float avgROI = 0;
            foreach (Worker w in workers) {
                foreach (string stat in statNames) {
                    results[stat] += w.StatByName(stat).value;
                }
                avgROI += WorkerROI(w);
            }

            foreach (string stat in statNames) {
                results[stat] /= workers.Count;
            }
            results["Average ROI"] = avgROI/workers.Count;
        } else {
            results["Average ROI"] = 0;
        }

        return results;
    }

    // Calculate the "value" of a worker.
    protected float WorkerROI(Worker w) {
        return (w.productivity.value + ((w.charisma.value + w.creativity.value + w.cleverness.value)/3))/(w.salary+w.happiness.value);
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


