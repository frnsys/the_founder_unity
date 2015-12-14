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
    public float monthlyPay {
        get { return salary/12; }
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
    // Recent offers the player has made.
    public int offMarketTime;
    public int recentPlayerOffers;

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
        recentPlayerOffers = 0;
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
    public string bio {
        get { return worker.bio; }
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

    private string RollStat() {
        // Selects one stat at random, weighted by value
        float total = charisma + creativity + cleverness;
        float rand = Random.Range(0f, total);
        string[] stats = new string[] {"Charisma", "Creativity", "Cleverness"};

        float acc = 0;
        string selected = null;
        foreach (string stat in stats) {
            acc += StatByName(stat);
            if (acc >= rand) {
                selected = stat;
                break;
            }
        }
        return selected;
    }

    private bool RollBreakthrough() {
        // Rolls to see if employee has a breakthrough or not
        // Need to ensure that happiness can't go over 100
        float breakthrough_prob = happiness / 200;
        return Random.value < breakthrough_prob;
    }

    public Stat Work(Product p) {
        Stat stat;
        if (RollBreakthrough()) {
            stat = new Stat("Breakthrough", Randomize(
                Mathf.Max(creativity, cleverness, charisma)
            ));
        } else {
            string statName = RollStat();
            float val = StatByName(statName);

            // worst this can be is 0.5
            float messUpProb = (1 - Mathf.Min(1f, val/p.difficulty)) * 0.5f;
            if (Random.value < messUpProb) {
                // at minimum, this is 1
                stat = new Stat(statName, -Mathf.Max(1f, Randomize(val) * 0.75f));
            } else {
                stat = new Stat(statName, Randomize(val));
            }
        }
        p.Develop(stat);
        return stat;
    }

    private float Randomize(float value) {
        return Mathf.Max(1, (0.5f + Random.value) * value);
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


