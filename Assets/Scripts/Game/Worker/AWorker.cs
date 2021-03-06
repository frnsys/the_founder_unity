/*
 * An mutable instance of a Worker
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class AWorker : ScriptableObject {
    [SerializeField]
    private Worker worker;

    // The worker's avatar in the game world.
    [HideInInspector, System.NonSerialized]
    public GameObject avatar;

    public float salary;
    public float hiringFee {
        get {
            // For robots, the baseMinSalary is the one-time cost.
            return worker.robot ? worker.baseMinSalary : salary * 0.1f;
        }
    }
    public float score {
        get {
            return cleverness +
                   creativity +
                   charisma +
                   productivity +
                   happiness;
        }
    }

    public List<Worker.Preference> personalInfo {
        get { return worker.personalInfo; }
    }
    public List<string> personalInfos;
    public List<string> knownPersonalInfos;

    // Returns a "clone" of this worker.
    public AWorker Clone() {
        if (!worker.robot) {
            AWorker w = new AWorker(worker);
            w.salary = salary;
            return w;
        } else {
            return null;
        }
    }

    public float MinSalaryForCompany(Company c) {
        float minSalary = worker.baseMinSalary;
        // If the employee is currently hired, i.e. has a salary > 0,
        // their minimum acceptable salary depends on their happiness at their current company.
        if (salary > 0) {
            float adjustedDiff = 0;
            if (c.workers.Count > 0) {
                // First calculate the effect the current company has on their happiness.
                float current = happiness - worker.happiness;

                // Get the average happiness at the hiring company.
                float avgHappiness = c.AggregateWorkerStat("Happiness")/c.workers.Count;

                // Estimate how much happier this employee would be at the hiring company.
                float diff = (avgHappiness - worker.happiness) - current;

                // It's difficult changing jobs, so slightly lower the expected happiness.
                adjustedDiff = diff - 5;
            }

            // TO DO tweak this.
            minSalary = Mathf.Max(0, salary + (-adjustedDiff/10 * 1000));
        }
        return minSalary * GameManager.Instance.wageMultiplier;
    }
    public float MinSalaryForCompany() {
        float minSalary = worker.baseMinSalary;
        if (salary > 0) {
            float diff = 0;
            diff = happiness - worker.happiness;
            minSalary = Mathf.Max(0, salary + (diff/10 * 1000));
        }
        return minSalary * GameManager.Instance.wageMultiplier;
    }

    // How many weeks the worker is off the job market for.
    // Turns the player has used so far.
    public int offMarketTime;
    public float leaveProb;

    public float happiness;
    public float productivity;
    public float charisma;
    public float creativity;
    public float cleverness;

    public AWorker(Worker w) {
        worker = w;

        happiness     = w.happiness;
        productivity  = w.productivity;
        charisma      = w.charisma;
        creativity    = w.creativity;
        cleverness    = w.cleverness;

        offMarketTime = 0;
        leaveProb = Worker.baseLeaveProb;

        knownPersonalInfos = new List<string>();
        personalInfos = new List<string>();
        foreach (Worker.Preference p in worker.personalInfo) {
            string[] descs = Worker.prefToDescMap[p];
            personalInfos.Add(descs[Random.Range(0, descs.Length)]);
        }
    }

    public string name {
        get { return worker.name; }
        set {} // necessary for serialization
    }
    public string title {
        get { return worker.title; }
    }
    public bool robot {
        get { return worker.robot; }
    }
    public Material material {
        get { return worker.material; }
    }
    public string description {
        get { return worker.description; }
    }
    public string[] bio {
        get { return worker.bio.Split('|'); }
    }
    public float baseMinSalary {
        get { return worker.baseMinSalary; }
    }

    public float StatByName(string name) {
        switch (name) {
            case "Happiness":
                return happiness;
            case "Productivity":
                return productivity;
            case "Charisma":
                return charisma;
            case "Creativity":
                return creativity;
            case "Cleverness":
                return cleverness;
            default:
                return 0;
        }
    }

    public void ApplyBuffs(List<StatBuff> buffs) {
        foreach (StatBuff buff in buffs) {
            ModifyStat(buff.name, buff.value);
        }
    }

    public void RemoveBuffs(List<StatBuff> buffs) {
        foreach (StatBuff buff in buffs) {
            ModifyStat(buff.name, -buff.value);
        }
    }

    public void ModifyStat(string name, float val) {
        switch (name) {
            case "Happiness":
                happiness += val;
                break;
            case "Productivity":
                productivity += val;
                break;
            case "Charisma":
                charisma += val;
                break;
            case "Creativity":
                creativity += val;
                break;
            case "Cleverness":
                cleverness += val;
                break;
            default:
                break;
        }
    }
}


