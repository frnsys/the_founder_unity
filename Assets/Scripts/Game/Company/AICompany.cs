/*
 * Companies managed by AI for you to compete against.
 *
 * The AI company has a minimal set of things it actually does
 * to simulate competition with the player.
 * Everything else is faked.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AICompany : ScriptableObject {
    public bool disabled = false;
    public int productLimit = 5;

    public string slogan;
    public string description;
    public EffectSet bonuses;
    public List<Vertical> specialtyVerticals;
    public List<ProductType> specialtyProductTypes;
    public List<Worker> workers;
    public List<Founder> founders;
    public int sizeLimit = 5;

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

    // Call `Init()` if creating a new AICompany from scratch.
    public new AICompany Init() {
        // Initialize stuff.
        founders = new List<Founder>();
        workers = new List<Worker>();
        bonuses = new EffectSet();

        return this;
    }

    // Call `Setup()` if instantiating an AICompany from an asset.
    public void Setup() {
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
        DecideWorkers();
    }


    // ===============================================
    // Product Management ============================
    // ===============================================

    public Product CreateProductForTypes(List<ProductType> pts) {
        int design = Random.Range(0, designSkill) + designSkill/2;
        int engineering = Random.Range(0, engineeringSkill) + engineeringSkill/2;
        int marketing = Random.Range(0, marketingSkill) + marketingSkill/2;
        Product product = ScriptableObject.CreateInstance<Product>();
        product.Init(pts, design, marketing, engineering);
        return product;
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
                    float minSalary = w.MinSalaryForCompany();
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

    public void FireWorker(Worker worker) {
        worker.salary = 0;
        workers.Remove(worker);
    }

    public bool HireWorker(Worker worker) {
        if (workers.Count < sizeLimit) {
            workers.Add(worker);
            return true;
        }
        return false;
    }
}
