/*
 * Companies managed by AI for you to compete against.
 *
 * The AI company has a minimal set of things it actually does
 * to simulate competition with the player.
 *
 * Everything else is faked:
 * - No acquisitions
 * - Ignores infrastructure
 *   - can build products w/o it
 *   - instead, there is a max product limit
 * - Events don't happen to them
 * - No perks - instead, only the fixed bonuses at the start
 * - Locations and markets are fixed
 * - Opinion & publicity increase at fixed rates
 *   - no opinion czar
 *   - over time, the player company will overtake these rates
 * - No research
 *   - AI companies have access to all product types from the start
 * - Verticals are fixed
 * - Recruiting is ignored - company has access to all workers
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class AICompany : Company {
    public bool disabled = false;
    public int productLimit = 5;

    public string description;
    public UnlockSet unlocked;
    public EffectSet bonuses;
    public List<Vertical> specialtyVerticals;
    public List<ProductType> specialtyProductTypes;
    public List<Location> startLocations;

    public int designSkill;
    public int engineeringSkill;
    public int marketingSkill;


    // To easily keep track of all AI companies.
    public static List<AICompany> all = new List<AICompany>();

    public static List<AICompany> LoadAll() {
        // Load companies as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<AICompany>("Companies").ToList().Select(c => {
                AICompany company = Instantiate(c) as AICompany;
                company.name = c.name;
                return company;
        }).ToList();
    }

    // Convenience method for locating the in-game version
    // of an AICompany from an asset version.
    public static AICompany Find(AICompany aic) {
        return Find(aic.name);
    }
    public static AICompany Find(string name) {
        return all.Where(a => a.name == name).First();
    }

    public AICompany(string name_) : base(name_) {
        name = name_;
    }

    // Call `Init()` if creating a new AICompany from scratch.
    public new AICompany Init() {
        base.Init();

        // Initialize stuff.
        unlocked = new UnlockSet();
        bonuses = new EffectSet();
        startLocations = new List<Location>();

        return this;
    }

    // Call `Setup()` if instantiating an AICompany from an asset.
    public void Setup() {
        foreach (Location l in startLocations) {
            // Give the company enough to open them.
            cash.baseValue += l.cost;
            ExpandToLocation(l);
        }

        specialtyVerticals = new List<Vertical>();
        specialtyProductTypes = new List<ProductType>();
        foreach (ProductEffect pe in bonuses.productEffects) {
            foreach (Vertical v in pe.verticals) {
                if (!specialtyVerticals.Contains(v))
                    specialtyVerticals.Add(v);
            }
            foreach (ProductType pt in pe.productTypes) {
                if (!specialtyProductTypes.Contains(pt))
                    specialtyProductTypes.Add(pt);
            }
        }
    }


    public void Decide() {
        Debug.Log("AI Company " + name + " is deciding...");
        DecideProducts();
        DecideWorkers();
    }


    // ===============================================
    // Product Management ============================
    // ===============================================

    private void DecideProducts() {
        // Cleanup any "stale" products.
        for (int i=0; i < activeProducts.Count; i++) {
            if (activeProducts[i].lastRevenue == 0)
                ShutdownProduct(activeProducts[i]);
        }

        // AI companies basically exist to compete with the player.
        // Ignore products for which the company already has a matching product.
        List<Product> candidates = new List<Product>();
        foreach (Product p in GameManager.Instance.playerCompany.activeProducts
                .Where(p => FindMatchingProducts(p.productTypes).Count == 0)) {
                candidates.Add(p);
        }

        // If we found some candidates,
        if (candidates.Count > 0) {
            // Rank the candidates and pick one.
            foreach (Product p in candidates.OrderBy(p => ScoreProduct(p))) {
                Debug.Log(name + " is starting a new competing product...");

                int design = Random.Range(0, designSkill) + designSkill/2;
                int engineering = Random.Range(0, engineeringSkill) + engineeringSkill/2;
                int marketing = Random.Range(0, marketingSkill) + marketingSkill/2;
                StartNewProduct(p.productTypes, design, engineering, marketing);
                return;
            }

        // If no candidates,
        // create a random specialty product.
        } else {
            // TO DO avoid creating duplicate products.
            Debug.Log(name + " is starting a new product...");

            int design = Random.Range(0, designSkill) + designSkill/2;
            int engineering = Random.Range(0, engineeringSkill) + engineeringSkill/2;
            int marketing = Random.Range(0, marketingSkill) + marketingSkill/2;
            StartNewProduct(RandomSpecialtyProduct(), design, engineering, marketing);
        }
    }

    private float ScoreProduct(Product p) {
        float score = p.revenueEarned/p.timeSinceLaunch;
        foreach (ProductType pt in p.productTypes) {
            if (specialtyProductTypes.Contains(pt))
                score *= 1.2f;
        }
        foreach (Vertical v in p.requiredVerticals) {
            if (specialtyVerticals.Contains(v))
                score *= 1.2f;
        }

        return score;
    }

    // Generate a random product combo based on this company's specialties. If no specialties are available for the aspect, a random unlocked one is chosen.
    private List<ProductType> RandomSpecialtyProduct() {
        List<ProductType> pts = new List<ProductType>();

        // TO DO: it's possible that the company creates a product of two of the same product type, it probably shouldn't be able to do this.
        while (pts.Count < 2) {
            if (specialtyProductTypes.Count > 0) {
                pts.Add(specialtyProductTypes[Random.Range(0, specialtyProductTypes.Count)]);
            } else {
                pts.Add(unlocked.productTypes[Random.Range(0, unlocked.productTypes.Count)]);
            }
        }

        return pts;
    }


    // ===============================================
    // Worker Management =============================
    // ===============================================

    private void DecideWorkers() {
        // Get average ROI of all workers.
        float avgROI = 0;
        if (workers.Count > 0) {
            avgROI = workers.Average(w => WorkerROI(w));

            // Review all hired workers (this should not include the founder(s)).
            List<Worker> toFire = new List<Worker>();
            for (int i = workers.Count - 1; i >= 0; i--) {
                float ROI = WorkerROI(workers[i]);
                if (ROI < avgROI * 0.8f) {
                    Debug.Log(string.Format("{0} is firing {1}...", name, workers[i].name));
                    FireWorker(workers[i]);
                }
            }
        }

        // AI companies will try to poach from other AI companies,
        // to drive up salaries.
        if (workers.Count < sizeLimit) {
            List<Worker> candidates = new List<Worker>();
            foreach (AICompany c in all.Where(x => x != this)) {
                foreach (Worker w in c.workers) {
                    if (WorkerROI(w) > avgROI) {
                        candidates.Add(w);
                    }
                }
            }

            // Also consider any worker in the job market.
            foreach (Worker w in GameManager.Instance.workerManager.AvailableWorkersForAICompany(this)) {
                if (WorkerROI(w) > avgROI) {
                    candidates.Add(w);
                }
            }

            // Rank the candidates and hire one.
            foreach (Worker w in candidates.OrderBy(w => WorkerROI(w))) {
                if (workers.Count < sizeLimit) {
                    // Fake an offer.
                    // It may be too low, (in which case, no hiring happens).
                    // Or it may be too high, in which case wages get driven up.
                    float minSalary = w.MinSalaryForCompany(this);
                    float offer = minSalary * (Random.value + 0.5f);
                    if (offer >= minSalary & Random.value < 0.2f) {
                        Debug.Log(string.Format("{0} is hiring {1} for {2:C0} (min was {3:C0})...", name, w.name, offer, minSalary));
                        GameManager.Instance.workerManager.HireWorker(w, this);
                        break;
                    }
                } else {
                    break;
                }
            }
        }
    }

    // Calculate the "value" of a worker.
    private float WorkerROI(Worker w) {
        return (w.productivity.value + ((w.charisma.value + w.creativity.value + w.cleverness.value)/3))/(w.salary+w.happiness.value);
    }
}
