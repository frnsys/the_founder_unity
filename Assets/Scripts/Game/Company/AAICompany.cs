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
public class AAICompany : ScriptableObject {
    [SerializeField]
    private AICompany aic;
    public bool disabled = false;
    public List<AWorker> workers;
    public List<Vertical> specialtyVerticals;
    public List<ProductType> specialtyProductTypes;

    public AAICompany(AICompany c) {
        workers = new List<AWorker>();
        aic = c;
        disabled = c.disabled;
    }

    // Call `Setup()` if instantiating an AAICompany for a new game.
    public void Setup() {
        specialtyVerticals = new List<Vertical>();
        specialtyProductTypes = new List<ProductType>();
        foreach (ProductEffect pe in aic.bonuses.productEffects) {
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

    public string name {
        get { return aic.name; }
        set {} // for serialization
    }
    public Worker founder {
        get { return aic.founder; }
        set { aic.founder = value; }
    }
    public AICompany aiCompany {
        get { return aic; }
    }

    // ===============================================
    // Product Management ============================
    // ===============================================

    public Product CreateProductForTypes(List<ProductType> pts) {
        int design = Random.Range(0, aic.designSkill) + aic.designSkill/2;
        int engineering = Random.Range(0, aic.engineeringSkill) + aic.engineeringSkill/2;
        int marketing = Random.Range(0, aic.marketingSkill) + aic.marketingSkill/2;
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
            List<AWorker> toFire = workers.Where(w => WorkerROI(w) < avgROI * 0.8f).ToList();
            for (int i = toFire.Count - 1; i >= 0; i--) {
                Debug.Log(string.Format("{0} is firing {1}...", name, workers[i].name));
                FireWorker(toFire[i]);
            }
        }

        // AI companies will try to poach from other AI companies,
        // to drive up salaries.
        if (workers.Count < aic.sizeLimit) {
            List<AWorker> candidates = new List<AWorker>();
            foreach (AAICompany c in AICompany.all.Where(x => x != this)) {
                candidates.AddRange(c.workers.Where(w => WorkerROI(w) > avgROI));
            }
            candidates.AddRange(
                GameManager.Instance.workerManager.AllWorkers.Where(w => !workers.Contains(w) && WorkerROI(w) > avgROI)
            );

            // Rank the candidates and hire one.
            foreach (AWorker w in candidates.OrderBy(w => WorkerROI(w))) {
                if (workers.Count < aic.sizeLimit) {
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
    private float WorkerROI(AWorker w) {
        return (w.productivity + ((w.charisma + w.creativity + w.cleverness)/3))/(w.salary+w.happiness);
    }

    public void FireWorker(AWorker worker) {
        worker.salary = 0;
        workers.Remove(worker);
    }

    public bool HireWorker(AWorker worker) {
        if (workers.Count < aic.sizeLimit) {
            workers.Add(worker);
            return true;
        }
        return false;
    }
}
