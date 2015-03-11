/*
 * Manages workers in the world.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class WorkerManager : MonoBehaviour {
    private GameData data;

    public void Load(GameData d) {
        data = d;
    }

    public IEnumerable<Worker> AvailableWorkers {
        get {
            return AllWorkers.Where(w => w.offMarketTime == 0 && !GameManager.Instance.playerCompany.workers.Contains(w));
        }
    }

    public Company EmployerForWorker(Worker w) {
        return GameManager.Instance.allCompanies.Where(c => c.workers.Contains(w)).SingleOrDefault();
    }

    // The canonical pool of workers in the game.
    public IEnumerable<Worker> AllWorkers {
        get {
            return new List<Worker>(data.unemployed).Concat(Employed);
        }
    }

    public List<Worker> WorkersForRecruitment(Recruitment r) {
        // This randomly sorts the available workers,
        // takes the first 10, and then randomly selects them based on their
        // scores and the recruitment strategy's target score.
        return AvailableWorkers.OrderBy(i => Random.value).Take(10)
            .Where(w => w.robot == r.robots && Random.value < 1 - Mathf.Abs(w.score - r.targetScore)/r.targetScore).ToList();
    }

    // If a worker is at a company,
    // that instance is considered the canonical one.
    public IEnumerable<Worker> Employed {
        get { return GameManager.Instance.allCompanies.SelectMany(c => c.workers); }
    }

    public bool HireWorker(Worker w) {
        return HireWorker(w, GameManager.Instance.playerCompany);
    }

    public bool HireWorker(Worker w, Company c) {
        Company employer = EmployerForWorker(w);
        if (c.HireWorker(w)) {
            if (data.unemployed.Contains(w)) {
                data.unemployed.Remove(w);

            // Poached employee.
            } else if (employer != null) {
                // Need to juggle the salary, b/c
                // firing a worker resets it to 0.
                float salary = w.salary;
                employer.FireWorker(w);
                w.salary = salary;
            }
            return true;
        }
        return false;
    }

    public void FireWorker(Worker w) {
        FireWorker(w, GameManager.Instance.playerCompany);
    }
    public void FireWorker(Worker w, Company c) {
        data.unemployed.Add(w);
        c.FireWorker(w);
    }
}


