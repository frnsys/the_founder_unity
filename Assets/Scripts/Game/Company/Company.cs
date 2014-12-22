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
    public Stat research;

    public float researchInvestment = 1000;

    public Company(string name_) {
        name = name_;
    }

    public List<Technology> technologies;

    public virtual void Awake() {
        // Default values.
        cash = new Stat("Cash", 100000);
        research = new Stat("Research", 1);
        lastMonthCosts = 0;
        lastMonthRevenue = 0;
        baseSizeLimit = 5;
        _items = new List<Item>();

        founders = new List<Founder>();
        _workers = new List<Worker>();
        _locations = new List<Location>();
        _verticals = new List<Vertical>() {
            Vertical.Load("Information")
        };
        _infrastructure = new Infrastructure();
        technologies = new List<Technology>();

        baseInfrastructureCapacity = new Infrastructure();
        baseInfrastructureCapacity[Infrastructure.Type.Datacenter] = 4;
        baseInfrastructureCapacity[Infrastructure.Type.Studio]     = 1;

        // Keep track for a year.
        PerfHistory = new PerformanceHistory(12);
        ProductPerfHistory = new PerformanceHistory(12);
        WorkerPerfHistory = new PerformanceHistory(12);
        ProductsReleased = new List<Product>();

        // Keep track for 10 years.
        AnnualPerfHistory = new PerformanceHistory(10);
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    public int baseSizeLimit;
    public int sizeLimit {
        // You can manage 5 employees at your HQ, other locations are managed by one employee.
        get { return baseSizeLimit + locations.Count; }
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

    public IEnumerable<Worker> allWorkers {
        get { return _workers.Concat(founders.Cast<Worker>()); }
    }

    public bool HireWorker(Worker worker) {
        if (_workers.Count < sizeLimit && Pay(worker.salary)) {
            foreach (Item item in _items) {
                worker.ApplyItem(item);
            }

            _workers.Add(worker);

            // Update the progress required for developing products.
            foreach (Product p in developingProducts) {
                p.requiredProgress = p.TotalProgressRequired(this);
            }

            return true;
        }
        return false;
    }
    public void FireWorker(Worker worker) {
        foreach (Item item in _items) {
            worker.RemoveItem(item);
        }

        // Reset the salary.
        worker.salary = 0;

        _workers.Remove(worker);

        // Update the progress required for developing products.
        foreach (Product p in developingProducts) {
            p.requiredProgress = p.TotalProgressRequired(this);
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

    public bool ExpandToLocation(Location l) {
        if (Pay(l.cost)) {
            _locations.Add(l);

            // Note this doesn't apply unlock effects...for now assuming locations don't have those.
            ApplyEffectSet(l.effects);
            return true;
        }
        return false;
    }


    // ===============================================
    // Vertical Management ===========================
    // ===============================================

    [SerializeField]
    private List<Vertical> _verticals;
    public ReadOnlyCollection<Vertical> verticals {
        get { return _verticals.AsReadOnly(); }
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

    public List<Product> products = new List<Product>();
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

    // Products grouped by state.
    public List<Product> sortedProducts {
        get {
            return developingProducts.Concat(activeProducts).Concat(retiredProducts).ToList();
        }
    }

    public void StartNewProduct(List<ProductType> pts, int design, int marketing, int engineering) {
        Product product = ScriptableObject.CreateInstance<Product>();
        product.Init(pts, design, marketing, engineering, this);

        // Apply any applicable items to the new product.
        // TO DO: should this be held off until after the product is completed?
        foreach (Item item in _items) {
            foreach (ProductEffect pe in item.effects.products) {
                if (IsEligibleForEffect(product, pe)) {
                    product.ApplyBuff(pe.buff);
                }
            }
        }

        products.Add(product);
    }

    private bool IsEligibleForEffect(Product p, ProductEffect pe) {
        // If the product effect is indiscriminate (i.e. doesn't specify any product types or verticals), it applies to every product.
        // Otherwise, a product must contain at least one of the specified effect's product types or verticals.
        if (pe.verticals.Count == 0 && pe.productTypes.Count == 0)
            return true;
        else if (pe.verticals.Count == 0 && pe.productTypes.Intersect(p.productTypes).Any())
            return true;
        else if (pe.productTypes.Count == 0 && pe.verticals.Intersect(p.requiredVerticals).Any())
            return true;
        else if (pe.productTypes.Intersect(p.productTypes).Any() && pe.verticals.Intersect(p.requiredVerticals).Any())
            return true;
        else
            return false;
    }

    public void DevelopProducts() {
        List<Product> inDevelopment = products.FindAll(p => p.state == Product.State.DEVELOPMENT);
        foreach (Product product in inDevelopment) {
            DevelopProduct(product);
        }
    }

    public void HarvestProducts(float elapsedTime) {
        List<Product> launched = products.FindAll(p => p.state == Product.State.LAUNCHED);

        float newRevenue = 0;
        foreach (Product product in launched) {
            newRevenue += product.Revenue(elapsedTime);
        }
        cash.baseValue += newRevenue;
        lastMonthRevenue += newRevenue;
    }

    public void DevelopProduct(Product product) {
        float progress = 0;

        foreach (Worker worker in allWorkers) {
            // A bit of randomness to make things more interesting.
            progress += worker.productivity.value * Random.Range(0.90f, 1.05f);
        }

        bool completed = product.Develop(progress);
        if (completed) {
            ProductsReleased.Add(product);

            // The product's effects are applied by the GameManager.
        }
    }

    public void ShutdownProduct(Product product) {
        foreach (Item item in _items) {
            foreach (ProductEffect pe in item.effects.products) {
                if (IsEligibleForEffect(product, pe)) {
                    product.RemoveBuff(pe.buff);
                }
            }
        }
        product.Shutdown();
    }

    public void ApplyEffectSet(EffectSet es) {
        ApplyBuffs(es.company);

        // TO DO this needs to apply bonuses to new workers as well.
        foreach (Worker worker in workers) {
            worker.ApplyBuffs(es.workers);
        }

        // TO DO this needs to apply bonuses to new products as well.
        foreach (ProductEffect pe in es.products) {
            ApplyProductEffect(pe);
        }
    }

    public void ApplyProductEffect(ProductEffect effect) {
        List<Product> matchingProducts = FindMatchingProducts(effect.productTypes);
        foreach (Product product in matchingProducts) {
            product.ApplyBuff(effect.buff);
        }
    }
    public void RemoveProductEffect(ProductEffect effect) {
        List<Product> matchingProducts = FindMatchingProducts(effect.productTypes);
        foreach (Product product in matchingProducts) {
            product.RemoveBuff(effect.buff);
        }
    }


    // Given an item, find the list of currently active products that
    // match at least one of the item's product types.
    protected List<Product> FindMatchingProducts(List<ProductType> productTypes) {
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
    public void PayMonthly() {
        float toPay = 0;
        foreach (Worker worker in workers) {
            toPay += worker.salary;
        }

        foreach (Location loc in locations) {
            toPay += loc.cost;
        }

        toPay += infrastructure.cost;
        toPay += researchInvestment;

        cash.baseValue -= toPay;

        // Reset month's costs with this month's costs.
        lastMonthCosts = toPay;

        // Also reset month's revenues.
        lastMonthRevenue = 0;
    }

    public bool Pay(float cost) {
        if (cash.baseValue - cost >= 0) {
            cash.baseValue -= cost;
            lastMonthCosts += cost;
            return true;
        }
        return false;
    }



    // ===============================================
    // Item Management ===============================
    // ===============================================

    public List<Item> _items;
    public ReadOnlyCollection<Item> items {
        get { return _items.AsReadOnly(); }
    }

    public bool BuyItem(Item item) {
        if (Pay(item.cost)) {
            _items.Add(item);

            foreach (ProductEffect pe in item.effects.products) {
                ApplyProductEffect(pe);
            }

            foreach (Worker worker in _workers) {
                worker.ApplyItem(item);
            }

            return true;
        }
        return false;
    }

    public void RemoveItem(Item item) {
        _items.Remove(item);

        foreach (ProductEffect pe in item.effects.products) {
            RemoveProductEffect(pe);
        }

        foreach (Worker worker in _workers) {
            worker.RemoveItem(item);
        }
    }

    // ===============================================
    // Infrastructure Management =====================
    // ===============================================

    [SerializeField]
    private Infrastructure _infrastructure;
    public Infrastructure infrastructure {
        get { return _infrastructure; }
    }

    public Infrastructure baseInfrastructureCapacity;

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
                return baseInfrastructureCapacity + locationCapacities.Aggregate((x,y) => x + y);
            return baseInfrastructureCapacity;
        }
    }

    // Infrastructure capacity which is unused.
    public Infrastructure availableInfrastructureCapacity {
        get {
            return infrastructureCapacity - infrastructure;
        }
    }

    public bool BuyInfrastructure(Infrastructure i) {
        if (HasCapacityFor(i) && Pay(i.cost)) {
            _infrastructure += i;
            return true;
        }
        return false;
    }

    public void DestroyInfrastructure(Infrastructure i) {
        _infrastructure -= i;
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
    protected PerformanceHistory AnnualPerfHistory;
    protected List<Product> ProductsReleased;

    // These data are sampled every month.
    private PerformanceDict SamplePerformance() {
        return new PerformanceDict {
            {"Month Revenue", lastMonthRevenue},
            {"Month Costs", lastMonthCosts}
        };
    }

    // Annually, aggregate data for the past year is collected.
    public List<PerformanceDict> CollectAnnualPerformanceData() {
        PerformanceDict results = new PerformanceDict();
        results["Annual Revenue"] = PerfHistory.Sum(x => x["Month Revenue"]);
        results["Annual Costs"] = PerfHistory.Sum(x => x["Month Costs"]);

        float avgPROI = 0;
        foreach (Product p in ProductsReleased) {
            // TO DO this could be more detailed.
            avgPROI += p.revenueEarned/p.points;
        }
        results["Product ROI"] = avgPROI/ProductsReleased.Count;
        AnnualPerfHistory.Enqueue(results);

        // Reset the products released for the new year.
        ProductsReleased.Clear();

        // Compare this year's performance to last years (if available).
        PerformanceDict deltas = new PerformanceDict();
        if (AnnualPerfHistory.Count > 1) {
            // Last year is the second to last element.
            PerformanceDict lastYear = (PerformanceDict)AnnualPerfHistory.Skip(AnnualPerfHistory.Count - 2).Take(1);

            foreach (string key in results.Keys) {
                deltas[key] = results[key]/lastYear[key] - 1f;
            }

        // Otherwise, everything improved by 100%!!!
        } else {
            foreach (string key in results.Keys) {
                deltas[key] = 1f;
            }
        }
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
        // (e.g. rent, salaries, etc) and value of items that contributed (normalized for their lifetime)
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


