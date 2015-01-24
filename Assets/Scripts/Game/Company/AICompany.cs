/*
 * Companies managed by AI for you to compete against.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// For convenience...
public class ProductCombo {
    public List<ProductType> pts;

    // The infrastructure this combo requires.
    public Infrastructure requiredInfrastructure {
        get {
            IEnumerable<Infrastructure> infras = pts.Select(i => i.requiredInfrastructure);
            if (infras.Count() > 0)
                return infras.Aggregate((x,y) => x + y);
            return new Infrastructure();
        }
    }

    public ProductCombo(List<ProductType> pts_) {
        pts = pts_;
    }
}


[System.Serializable]
public class AICompany : Company {
    public static List<AICompany> companies = new List<AICompany>();

    public AICompany(string name_) : base(name_) {
        name = name_;
    }

    public static List<AICompany> LoadAll() {
        // Load companies as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<AICompany>("Companies").ToList().Select(c => {
                AICompany company = Instantiate(c) as AICompany;
                company.name = c.name;
                company.Setup();
                return company;
        }).ToList();
    }

    // Bonuses the company gets for particular verticals.
    public EffectSet bonuses;
    public List<Vertical> specialtyVerticals;
    public List<ProductType> specialtyProductTypes;

    public List<Worker> startWorkers;
    public List<Product> startProducts;
    public List<Location> startLocations;

    // Call `Init()` if creating a new AICompany from scratch.
    public AICompany Init() {
        base.Init();

        // Initialize stuff.
        unlocked = new UnlockSet();
        bonuses = new EffectSet();

        startWorkers = new List<Worker>();
        startProducts = new List<Product>();
        startLocations = new List<Location>();
        specialtyVerticals = new List<Vertical>();
        specialtyProductTypes = new List<ProductType>();

        // Keep track of data for 2 years.
        PerfHistory = new PerformanceHistory(24);
        ProductPerfHistory = new PerformanceHistory(24);
        WorkerPerfHistory = new PerformanceHistory(24);

        return this;
    }

    // Call `Setup()` if instantiating an AICompany from an asset.
    public void Setup() {
        foreach (Worker w in startWorkers) {
            // Give the company enough to hire them.
            // Make sure to define the salary on the worker asset!
            cash.baseValue += w.salary;
            HireWorker(w);
        }

        foreach (Product p in startProducts) {
            // TO DO setup existing products
        }

        foreach (Location l in startLocations) {
            // Give the company enough to open them.
            cash.baseValue += l.cost;
            ExpandToLocation(l);
        }
    }

    // Influences how hostile they are against others.
    // 0 = friendly, unlikely to do anything hostile.
    // 1 = will try to sue the shit out of everyone.
    public float aggression;

    // Influences how willing they are to cooperate.
    // 0 = unwilling to cooperate on anything.
    // 1 = hellooo wage fixing!
    public float cooperativeness;

    // Influences how lucky they are.
    // <1 = a penalty to luck
    // >1 = a bonus to luck
    public float luck;

    public string description;

    // The stuff the company has access to, including
    // product aspects, workers, discoveries, etc...
    public UnlockSet unlocked;

    void OnEnable() {
        // TO DO we may need to manage the base unlocks on the game manager separately from the player company's unlocks, so that the AI companies don't just get everything the player has unlocked too.
        //unlocked.Unlock(bonuses.unlocks);
    }

    // Each turn the AI calculates the utility of all possible actions and does the one with the highest utility, provided it is above some threshold (?).
    public void Decide() {
        //Debug.Log("AI Company " + name + " has " + cash.value.ToString() + " cash.");

        // Only decide if there's enough history to act off of.
        if (PerfHistory.Count > 5) {
            Debug.Log("AI Company " + name + " is deciding...");

            // Decide which products to shutdown or sell.
            DecideShutdownProducts();
            DecideNewProducts();

            // Decide what to do with workers.
            //ReviewWorkers();
            //DecideHireWorkers();
        }

    }


    // ===============================================
    // Product Management ============================
    // ===============================================

    private void DecideNewProducts() {
        // Cache this value since we'll be re-using it a lot.
        // We include available _capacity_ since the company may want to purchase extra infrastructure
        // for the purpose of creating a new product.
        Infrastructure avInf = availableInfrastructure + availableInfrastructureCapacity;

        // If any available infrastructure...
        if (!avInf.isEmpty) {
            // The more aggressive the company, the more likely they are
            // to try and edge into a competitor's territory.
            if (Random.value < aggression) {

                // First look at what other companies have in the market.
                // Ones that a performing well are considered candidates
                // for copying.
                List<Product> candidates = new List<Product>();
                foreach (AICompany c in companies.Where(x => x != this)) {
                    foreach (Product p in c.products) {
                        // Ignore if the product isn't doing well
                        // or if the company already has this kind of product.
                        if (avInf >= p.requiredInfrastructure &&
                            ProductROI(p) > 1.2 && // this threshold value can really be anything. how to best determine it?
                            FindMatchingProducts(p.productTypes).Count == 0) {
                            candidates.Add(p);
                        }
                    }
                }

                // Rank the candidates.
                foreach (Product p in candidates.OrderBy(p => ScoreProduct(p))) {
                    // Be greedy ~ Create as many as possible.
                    if (p.requiredInfrastructure <= avInf) {
                        // Purchase new infrastructure if needed.
                        DecidePurchaseInfrastructure(p.requiredInfrastructure);

                        Debug.Log(name + " is starting a new competing product...");
                        // TO DO these should be actual design, marketing, engineering values.
                        StartNewProduct(p.productTypes, 3, 3, 3);

                        // Update this value, it has changed now that a new product was started.
                        avInf = availableInfrastructure + availableInfrastructureCapacity;
                    }
                }

            }

            // Less aggressive companies try to build products according to their strengths.
            // TO DO: this should be handled better to optimize infrastructure usage
            // and avoid creating duplicate products.
            ProductCombo pc = RandomSpecialtyProduct();
            if (pc.requiredInfrastructure <= avInf) {
                // Purchase new infrastructure if needed.
                DecidePurchaseInfrastructure(pc.requiredInfrastructure);

                Debug.Log(name + " is starting a new product...");

                // TO DO these should be actual design, marketing, engineering values.
                StartNewProduct(pc.pts, 3, 3, 3);

                // Update this value, it has changed now that a new product was started.
                avInf = availableInfrastructure + availableInfrastructureCapacity;
            }
        }
    }

    private void DecidePurchaseInfrastructure(Infrastructure inf) {
        Infrastructure avInf = availableInfrastructure;
        if (inf.AnyGreater(avInf)) {
            Infrastructure neededInf = inf - avInf;
            PurchaseInfrastructure(neededInf);
        }
    }

    // Buys infrastructure for locations that have space.
    private void PurchaseInfrastructure(Infrastructure inf) {
        Debug.Log(name + " is purchasing new infrastructure...");
        foreach (Location l in locations) {
            Infrastructure canPurchase = inf.Intersection(l.availableInfrastructureCapacity);
            BuyInfrastructure(canPurchase, l);
            inf = inf - canPurchase;

            if (inf.isEmpty)
                break;
        }
    }

    private void DecideShutdownProducts() {
        if (activeProducts.Count > 0) {
            float historicalAvgROI = ProductPerfHistory.Average(x => x["Average ROI"]);
            foreach (Product p in activeProducts) {
                // Some minimum amount of time a product is kept in market before considering shutdown.
                if (p.timeSinceLaunch > 40) {

                    // If this product is not generating any more revenue or
                    // is performing less than 1/3 of average, shutdown.
                    // TO DO this should maybe look at standard deviations instead?
                    if (p.lastRevenue == 0 || ProductROI(p) < historicalAvgROI * 0.33) {
                        Debug.Log(name + " is shutting down a product...");
                        p.Shutdown();
                    }
                }
            }
        }
    }

    // TO DO tweak this
    private float ScoreProduct(Product p) {
        float score = 0;
        foreach (ProductType pt in p.productTypes) {
            if (specialtyProductTypes.Contains(pt))
                score += 1;
        }
        foreach (Vertical v in p.requiredVerticals) {
            if (specialtyVerticals.Contains(v))
                score += 1;
        }

        score += ProductROI(p);

        return score;
    }

    // Generate a random product combo based on this company's specialties. If no specialties are available for the aspect, a random unlocked one is chosen.
    private ProductCombo RandomSpecialtyProduct() {
        List<ProductType> pts = new List<ProductType>();

        // Number of product types to use.
        int numProductTypes = Random.Range(1,2);

        // TO DO Note that it's possible that the AI company creates a product with a double product type.
        // (two of the same product type). Do we want this?
        while (pts.Count < numProductTypes) {
            if (specialtyProductTypes.Count > 0) {
                pts.Add(specialtyProductTypes[Random.Range(0, specialtyProductTypes.Count)]);
            } else {
                pts.Add(unlocked.productTypes[Random.Range(0, unlocked.productTypes.Count)]);
            }
        }

        return new ProductCombo(pts);
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    private void ReviewWorkers() {
        float historicalAvgROI = WorkerPerfHistory.Average(x => x["Average ROI"]);

        // Review all hired workers (this should not include the founder(s)).
        foreach (Worker w in workers) {
            // If this worker is performing significantly worse than the average, fire them.
            // If they are underperforming but could be doing better, upgrade them.
            // TO DO this should maybe look at standard deviations instead?
            float ROI = WorkerROI(w);
            if (ROI < historicalAvgROI * 0.33) {
                Debug.Log(name + " is firing a worker...");
                FireWorker(w);
            } else if (ROI < historicalAvgROI) {
                // TO DO UpgradeWorker hasn't been implemented yet.
                // TO DO needs to assess if the cost of upgrading a worker is worth it.
                // and if it can be afforded.
                //UpgradeWorker(w);
            }
        }
    }

    private void DecideHireWorkers() {
        if (workers.Count < sizeLimit) {
            float avgMonthlyRevenue = PerfHistory.Average(x => x["Month Revenue"]);
            float avgMonthlyCosts = PerfHistory.Average(x => x["Month Costs"]);
            float expectedMonthlyProfits = avgMonthlyRevenue - avgMonthlyCosts;

            // The more aggressive the company, the more likely they are
            // to try and poach a competitor's employees.
            List<Worker> candidates = new List<Worker>();
            if (Random.value < aggression) {
                foreach (AICompany c in companies.Where(x => x != this)) {
                    foreach (Worker w in c.workers) {
                        if (CanAffordWorker(w, expectedMonthlyProfits) &&
                            WorkerROI(w) > 1.2) { // this value could be anything
                            candidates.Add(w);
                        }
                    }
                }
            }

            // Also consider any worker in the job market.
            foreach (Worker w in GameManager.Instance.workerManager.AvailableWorkersForAICompany(this)) {
                if (CanAffordWorker(w, expectedMonthlyProfits) &&
                    WorkerROI(w) > 1.2) { // this value could be anything
                    candidates.Add(w);
                }
            }

            // Rank the candidates and hire as many as possible.
            foreach (Worker w in candidates.OrderBy(w => WorkerROI(w))) {
                // keep track of the new monthly costs associated with hiring new workers.
                float newMonthlyCosts = 0;

                if (workers.Count < sizeLimit &&
                    CanAffordWorker(w, expectedMonthlyProfits - newMonthlyCosts)) {
                    Debug.Log(name + " is hiring a worker...");
                    GameManager.Instance.workerManager.HireWorker(w, this);
                    newMonthlyCosts += w.salary;
                }
            }
        }
    }


    private bool CanAffordWorker(Worker w, float monthlyProfits) {
        return monthlyProfits - w.salary > 0;
    }


    // ===============================================
    // Asset Acquisition =============================
    // ===============================================

    // TO DO:
    // locate defecits that particular items/policies/perks could help with,
    // see if they are affordable against est. monthly costs and revenue


    // ===============================================
    // Research Management ===========================
    // ===============================================

    // TO DO:
    // somehow assess which technology is most worthwhile to pursue.



    /*
     * Possible actions:
     * =================
     *
     * Products:
     * - shutdown a product
     * - start a new product
     * - try and sell a product
     *
     * Workers:
     * - hire a worker
     * - fire a worker
     * - upgrade a worker
     *
     * Market:
     * - purchase items (policies, perks, etc)
     * - purchase minor companies
     *
     * Interact:
     * - propose some kind of deal (merger/wage or price fixing/etc)
     * - do something hostile (sue you)
     */
}
