/*
 * Companies managed by AI for you to compete against.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// For convenience...
public class ProductCombo {
    public ProductType pt;
    public Industry i;
    public Market m;

    // The number of product points this product requires.
    public int points {
        get { return pt.points + i.points + m.points; }
    }

    public ProductCombo(ProductType pt_, Industry i_, Market m_) {
        pt = pt_;
        i  = i_;
        m  = m_;
    }
}



public class AICompany : Company {
    public static List<AICompany> companies = new List<AICompany>();

    public AICompany(string name_) : base(name_) {
        name = name_;
    }

    // Bonuses the company gets for particular product aspects.
    public EffectSet bonuses;
    private List<ProductType> specialtyProductTypes;
    private List<Industry> specialtyIndustries;
    private List<Market> specialtyMarkets;

    public List<Worker> startWorkers;
    public List<Product> startProducts;

    public override void Awake() {
        base.Awake();

        // Initialize stuff.
        bonuses = new EffectSet();
        specialtyProductTypes = new List<ProductType>();
        specialtyIndustries = new List<Industry>();
        specialtyMarkets = new List<Market>();

        startWorkers = new List<Worker>();
        startProducts = new List<Product>();

        // Keep track of data for 2 years.
        PerfHistory = new PerformanceHistory(24);
        ProductPerfHistory = new PerformanceHistory(24);
        WorkerPerfHistory = new PerformanceHistory(24);
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
    [SerializeField]
    private UnlockSet unlocked;

    void OnEnable() {
        // Load up all the specialty product aspects.
        foreach (ProductEffect pe in bonuses.products) {
            foreach (ProductType i in pe.productTypes) {
                if (!specialtyProductTypes.Contains(i))
                    specialtyProductTypes.Add(i);
            }
            foreach (Industry i in pe.industries) {
                if (!specialtyIndustries.Contains(i))
                    specialtyIndustries.Add(i);
            }
            foreach (Market i in pe.markets) {
                if (!specialtyMarkets.Contains(i))
                    specialtyMarkets.Add(i);
            }
        }

        // TO DO we may need to manage the base unlocks on the game manager separately from the player company's unlocks, so that the AI companies don't just get everything the player has unlocked too.
        unlocked = bonuses.unlocks;
    }

    // Each turn the AI calculates the utility of all possible actions and does the one with the highest utility, provided it is above some threshold (?).
    public void Decide() {
        Debug.Log("AI Company " + name + " has " + cash.value.ToString() + " cash.");
        Debug.Log("AI Company " + name + " is deciding...");

        // Only decide if there's enough history to act off of.
        if (PerfHistory.Count > 5) {
            // Decide which products to shutdown or sell.
            DecideShutdownProducts();
            DecideNewProducts();

            // Decide what to do with workers.
            ReviewWorkers();
            DecideHireWorkers();
        }

    }



    // ===============================================
    // Product Management ============================
    // ===============================================

    private void DecideNewProducts() {
        // 3 is the min. PP required for a new product.
        if (availableProductPoints > 3) {

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
                        if (availableProductPoints > p.points &&
                            ProductROI(p) > 1.2 && // this threshold value can really be anything. how to best determine it?
                            MatchingProducts(p).Count == 0) {
                            candidates.Add(p);
                        }
                    }
                }

                // Rank the candidates.
                foreach (Product p in candidates.OrderBy(p => ScoreProduct(p))) {

                    // Create as many as possible.
                    if (p.points <= availableProductPoints) {
                        Debug.Log(name + " is starting a new competing product...");
                        StartNewProduct(p.productType, p.industry, p.market);
                    }
                }

            }

            // Less aggressive companies try to build products according to their strengths.
            // TO DO: this should be handled better to optimize PP usage
            // and avoid creating duplicate products.
            while (true) {
                ProductCombo pc = RandomSpecialtyProduct();
                if (pc.points <= availableProductPoints) {
                    Debug.Log(name + " is starting a new product...");
                    StartNewProduct(pc.pt, pc.i, pc.m);
                } else {
                    break;
                }
            }
        }
    }

    private void DecideShutdownProducts() {
        float historicalAvgROI = ProductPerfHistory.Average(x => x["Average ROI"]);
        foreach (Product p in activeProducts) {
            // Some minimum amount of time a product is kept in market before considering shutdown.
            // TO DO tweak this value.
            if (p.timeSinceLaunch > 1000) {

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

    // TO DO tweak this
    private float ScoreProduct(Product p) {
        float score = 0;
        if (specialtyProductTypes.Contains(p.productType))
            score += 1;
        if (specialtyIndustries.Contains(p.industry))
            score += 1;
        if (specialtyMarkets.Contains(p.market))
            score += 1;

        score += ProductROI(p);

        return score;
    }

    // Generate a random product combo based on this company's specialties. If no specialties are available for the aspect, a random unlocked one is chosen.
    private ProductCombo RandomSpecialtyProduct() {
        ProductType pt;
        Industry i;
        Market m;

        if (specialtyProductTypes.Count > 0) {
            pt = specialtyProductTypes[Random.Range(0, specialtyProductTypes.Count)];
        } else {
            pt = unlocked.productTypes[Random.Range(0, unlocked.productTypes.Count)];
        }

        if (specialtyIndustries.Count > 0) {
            i = specialtyIndustries[Random.Range(0, specialtyIndustries.Count)];
        } else {
            i = unlocked.industries[Random.Range(0, unlocked.industries.Count)];
        }

        if (specialtyMarkets.Count > 0) {
            m = specialtyMarkets[Random.Range(0, specialtyMarkets.Count)];
        } else {
            m = unlocked.markets[Random.Range(0, unlocked.markets.Count)];
        }
        return new ProductCombo(pt, i, m);
    }

    // Returns products which matches the aspect combo of another.
    private List<Product> MatchingProducts(Product p) {
        return products.Where(x =>
                x.productType == p.productType &&
                x.industry    == p.industry &&
                x.market      == p.market).ToList();
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
            foreach (Worker w in GameManager.Instance.availableWorkers) {
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
                    HireWorker(w);
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
    // pick a consultancy to hire
    // somehow assess which discovery is most worthwhile to pursue.



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
