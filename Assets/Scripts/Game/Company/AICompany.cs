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
using System.Collections;
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
        List<Product> candidates = new List<Product>();
        List<Product> stale = new List<Product>(products);

        // AI companies basically exist to compete with the player.
        // Ignore products for which the company already has a matching product.
        //foreach (Product p in GameManager.Instance.playerCompany.activeProducts
        foreach (Product p in GameManager.Instance.playerCompany.activeProducts) {
            Product matching = MatchingProduct(p);
            if (matching == null) {
                candidates.Add(p);
            } else {
                // Remove products from the stale list,
                // so that the only ones left over are ones to delete.
                stale.Remove(matching);
            }
        }

        if (candidates.Count > 0) {
            Debug.Log(name + " is starting a new competing product...");

            int design = Random.Range(0, designSkill) + designSkill/2;
            int engineering = Random.Range(0, engineeringSkill) + engineeringSkill/2;
            int marketing = Random.Range(0, marketingSkill) + marketingSkill/2;

            // Find a product to copy - but only copy products after
            // they have been in the market for a little while.
            Product product = candidates.Where(p => p.timeSinceLaunch >= 2000)
                    .OrderBy(p => ScoreProduct(p)).FirstOrDefault();
            if (product != null) {
                // Create it and launch it immediately.
                Product newProduct = StartNewProduct(product.productTypes , design, engineering, marketing);
                newProduct.Develop(newProduct.requiredProgress, this);
            }
        }

        // Cleanup any "stale" products.
        for (int i=stale.Count; i >= 0; i--) {
            products.Remove(stale[i]);
        }
    }

    private Product MatchingProduct(Product p) {
        // Only need to return the first, since AI companies will
        // create only one competing product for a combo.
        return activeProducts.FirstOrDefault(p_ => p_.comboID == p.comboID);
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

    // ===============================================
    // Worker Management =============================
    // ===============================================

    private void DecideWorkers() {
        // Get average ROI of all workers.
        float avgROI = 0;
        if (workers.Count > 0) {
            avgROI = workers.Average(w => WorkerROI(w));

            // Review all hired workers (this should not include the founder(s)).
            List<Worker> toFire = workers.Where(w => WorkerROI(w) < avgROI * 0.8f).ToList();
            for (int i = toFire.Count - 1; i >= 0; i--) {
                Debug.Log(string.Format("{0} is firing {1}...", name, workers[i].name));
                FireWorker(toFire[i]);
            }
        }

        // AI companies will try to poach from other AI companies,
        // to drive up salaries.
        if (workers.Count < sizeLimit) {
            List<Worker> candidates = new List<Worker>();
            foreach (AICompany c in all.Where(x => x != this)) {
                candidates.AddRange(c.workers.Where(w => WorkerROI(w) > avgROI));
            }
            candidates.AddRange(
                GameManager.Instance.workerManager.AllWorkers.Where(w => !workers.Contains(w) && WorkerROI(w) > avgROI)
            );

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
