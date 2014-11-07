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

// https://stackoverflow.com/a/5852926/1097920
public class FixedSizedQueue<T> : Queue<T> {
    public int Size { get; private set; }
    public FixedSizedQueue(int size) {
        Size = size;
    }

     public void Enqueue(T obj) {
        base.Enqueue(obj);
        T overflow;
        while (Count > Size)
            Dequeue();
     }
 }

public class AICompany : Company {
    public static List<AICompany> companies = new List<AICompany>();

    public AICompany(string name_) : base(name_) {
        name = name_;
    }

    // Bonuses the company gets for particular product aspects.
    public EffectSet bonuses = new EffectSet();
    private List<ProductType> specialtyProductTypes = new List<ProductType>();
    private List<Industry> specialtyIndustries = new List<Industry>();
    private List<Market> specialtyMarkets = new List<Market>();

    public List<Worker> startWorkers = new List<Worker>();
    public List<Product> startProducts = new List<Product>();

    // The personality tweaks many of the decision functions.
    public Personality personality = new Personality();

    public string description;

    // The stuff the company has access to, including
    // product aspects, workers, discoveries, etc...
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
        unlocked = bonuses.unlocks;
    }

    // Each turn the AI calculates the utility of all possible actions and does the one with the highest utility, provided it is above some threshold (?).
    public void Decide() {

        // Decide which products to shutdown or sell.
        DecideShutdownProducts();
        DecideNewProducts();

        // Decide what to do with workers.
        ReviewWorkers();
        DecideHireWorkers();
    }



    // ===============================================
    // Product Management ============================
    // ===============================================

    private void DecideNewProducts() {
        // 3 is the min. PP required for a new product.
        if (availableProductPoints > 3) {

            // The more aggressive the company, the more likely they are
            // to try and edge into a competitor's territory.
            if (Random.value < personality.aggression) {

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
                    StartNewProduct(pc.pt, pc.i, pc.m);
                } else {
                    break;
                }
            }
        }
    }

    private void DecideShutdownProducts() {
        float historicalAvgROI = PerfHistory.Sum(x => x["Average ROI"])/PerfHistory.Size;
        foreach (Product p in activeProducts) {
            // Some minimum amount of time a product is kept in market before considering shutdown.
            // TO DO tweak this value.
            if (p.timeSinceLaunch > 1000) {

                // If this product is not generating any more revenue or
                // is performing less than 1/3 of average, shutdown.
                // TO DO this should maybe look at standard deviations instead?
                if (p.lastRevenue == 0 || ProductROI(p) < historicalAvgROI * 0.33) {
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

    // Generate a random product combo based on this company's specialties.
    private ProductCombo RandomSpecialtyProduct() {
        ProductType pt = specialtyProductTypes[Random.Range(0, specialtyProductTypes.Count)];
        Industry i     = specialtyIndustries[Random.Range(0, specialtyIndustries.Count)];
        Market m       = specialtyMarkets[Random.Range(0, specialtyMarkets.Count)];
        return new ProductCombo(pt, i, m);
    }

    // Returns products which matches the aspect combo of another.
    private List<Product> MatchingProducts(Product p) {
        return products.Where(x =>
                x.productType == p.productType &&
                x.industry    == p.industry &&
                x.market      == p.market).ToList();
    }

    // Calculate the return on investment for a product,
    // normalized for time.
    private float ProductROI(Product p) {
        // TO DO tweak this
        // this should maybe also take into account cash invested
        // (e.g. rent, salaries, etc) and value of items that contributed (normalized for their lifetime)
        return (p.revenueEarned/p.timeSinceLaunch)/p.points;
    }



    // ===============================================
    // Worker Management =============================
    // ===============================================

    // Calculate the "value" of a worker.
    private float WorkerROI(Worker w) {
        return (w.productivity.value + ((w.charisma.value + w.creativity.value + w.cleverness.value)/3))/(w.salary+w.happiness.value);
    }

    private void ReviewWorkers() {
        float historicalAvgROI = WorkerPerfHistory.Sum(x => x["Average ROI"])/WorkerPerfHistory.Size;

        // Review all hired workers, except the founder obv.
        foreach (Worker w in workers.Where(x => !founders.Contains(x))) {
            // If this worker is performing significantly worse than the average, fire them.
            // If they are underperforming but could be doing better, upgrade them.
            // TO DO this should maybe look at standard deviations instead?
            float ROI = WorkerROI(w);
            if (ROI < historicalAvgROI * 0.33) {
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
            float avgMonthlyRevenue = PerfHistory.Sum(x => x["Month Revenue"])/PerfHistory.Size;
            float avgMonthlyCosts = PerfHistory.Sum(x => x["Month Costs"])/PerfHistory.Size;
            float expectedMonthlyProfits = avgMonthlyRevenue - avgMonthlyCosts;

            // The more aggressive the company, the more likely they are
            // to try and poach a competitor's employees.
            List<Worker> candidates = new List<Worker>();
            if (Random.value < personality.aggression) {
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


    // ===============================================
    // Performance Data ==============================
    // ===============================================

    // The AI Company must surveil its assets in order to make decisions.

    public void CollectPerformanceData() {
        PerfHistory.Enqueue(SamplePerformance());
        ProductPerfHistory.Enqueue(ProductAverages());
        WorkerPerfHistory.Enqueue(WorkerAverages());
    }

    // Keep track of company performance history as well to try and make decisions.
    private FixedSizedQueue<Dictionary<string, float>> PerfHistory = new FixedSizedQueue<Dictionary<string, float>>(24);
    private FixedSizedQueue<Dictionary<string, float>> ProductPerfHistory = new FixedSizedQueue<Dictionary<string, float>>(24);
    private FixedSizedQueue<Dictionary<string, float>> WorkerPerfHistory = new FixedSizedQueue<Dictionary<string, float>>(24);

    // These data are sampled every month.
    private Dictionary<string, float> SamplePerformance() {
        return new Dictionary<string, float> {
            {"Month Revenue", lastMonthRevenue},
            {"Month Costs", lastMonthCosts}
        };
    }

    private Dictionary<string, float> ProductAverages() {
        float avgROI = 0;
        foreach (Product p in activeProducts) {
            avgROI += ProductROI(p);
        }
        avgROI /= activeProducts.Count;

        return new Dictionary<string, float> {
            {"Average ROI", avgROI}
        };
    }

    // Aggregate averages of certain worker stats.
    private Dictionary<string, float> WorkerAverages() {
        Dictionary<string, float> results = new Dictionary<string, float> {
            {"Happiness", 0f},
            {"Productivity", 0f}
        };
        float avgROI = 0;
        foreach (Worker w in workers) {
            foreach (string stat in results.Keys) {
                results[stat] += w.StatByName(stat).value;
            }
            avgROI += WorkerROI(w);
        }

        foreach (string stat in results.Keys) {
            results[stat] /= workers.Count;
        }
        results["Average ROI"] = avgROI/workers.Count;

        return results;
    }


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
