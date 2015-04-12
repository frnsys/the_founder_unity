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

    public IEnumerable<AWorker> AvailableWorkers {
        get {
            return AllWorkers.Where(w => w.offMarketTime == 0 && !GameManager.Instance.playerCompany.workers.Contains(w));
        }
    }

    public Company EmployerForWorker(AWorker w) {
        if (GameManager.Instance.playerCompany.workers.Contains(w)) {
            return GameManager.Instance.playerCompany;
        }
        return null;
    }
    public AICompany AIEmployerForWorker(AWorker w) {
        return GameManager.Instance.otherCompanies.Where(c => c.workers.Contains(w)).SingleOrDefault();
    }

    // The canonical pool of workers in the game.
    public IEnumerable<AWorker> AllWorkers {
        get {
            return new List<AWorker>(data.unemployed).Concat(Employed);
        }
    }

    public List<AWorker> WorkersForRecruitment(Recruitment r) {
        IEnumerable<AWorker> ws = AvailableWorkers.Where(w => w.robot == r.robots && Random.value < 1 - Mathf.Abs(w.score - r.targetScore)/r.targetScore).OrderBy(i => Random.value);
        return ws.Take(Random.Range(1, ws.Count())).ToList();
    }

    // If a worker is at a company,
    // that instance is considered the canonical one.
    public IEnumerable<AWorker> Employed {
        get { return GameManager.Instance.otherCompanies.SelectMany(c => c.workers).Concat(GameManager.Instance.playerCompany.workers); }
    }

    public bool HireWorker(AWorker w) {
        Company employer = EmployerForWorker(w);
        AICompany aiEmployer = AIEmployerForWorker(w);

        if (GameManager.Instance.playerCompany.HireWorker(w)) {
            if (data.unemployed.Contains(w)) {
                data.unemployed.Remove(w);

            // Poached employee.
            } else if (employer != null) {
                // Need to juggle the salary, b/c
                // firing a worker resets it to 0.
                float salary = w.salary;
                employer.FireWorker(w);
                w.salary = salary;
            } else if (aiEmployer != null) {
                float salary = w.salary;
                aiEmployer.FireWorker(w);
                w.salary = salary;
            }
            return true;
        }
        return false;
    }

    public bool HireWorker(AWorker w, AICompany c) {
        Company employer = EmployerForWorker(w);
        AICompany aiEmployer = AIEmployerForWorker(w);

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
            } else if (aiEmployer != null) {
                float salary = w.salary;
                aiEmployer.FireWorker(w);
                w.salary = salary;
            }
            return true;
        }
        return false;
    }

    public void FireWorker(AWorker w) {
        data.unemployed.Add(w);
        GameManager.Instance.playerCompany.FireWorker(w);
    }
    public void FireWorker(AWorker w, AICompany c) {
        data.unemployed.Add(w);
        c.FireWorker(w);
    }
}


