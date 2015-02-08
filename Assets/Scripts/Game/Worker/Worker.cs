/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Worker : HasStats {
    public static List<Worker> LoadAll() {
        // Load workers as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<Worker>("Workers").ToList().Select(w => {
                Worker worker = Instantiate(w) as Worker;
                worker.name = w.name;
                return worker;
        }).ToList();
    }

    // The worker's avatar in the game world.
    public GameObject avatar;

    // Each worker has their own style!
    public Texture texture;

    // Later on, there are robotic workers.
    public bool robot = false;

    public float salary;
    public float hiringFee {
        get {
            // For robots, the baseMinSalary is the one-time cost.
            return robot ? baseMinSalary : salary * 0.1f;
        }
    }
    public float monthlyPay {
        get { return salary/12; }
    }
    public float score {
        get {
            return cleverness.value +
                   creativity.value +
                   charisma.value +
                   productivity.value +
                   happiness.value;
        }
    }

    public string bio;
    public string description;
    public string title;

    public float baseMinSalary;
    public float MinSalaryForCompany(Company c) {
        // If the employee is currently hired, i.e. has a salary > 0,
        // their minimum acceptable salary depends on their happiness at their current company.
        if (salary > 0) {
            float adjustedDiff = 0;
            if (c.workers.Count > 0) {
                // First calculate the effect the current company has on their happiness.
                float current = happiness.baseValue - happiness.value;

                // Get the average happiness at the hiring company.
                float avgHappiness = c.AggregateWorkerStat("Happiness")/c.workers.Count;

                // Estimate how much happier this employee would be at the hiring company.
                float diff = (avgHappiness - happiness.baseValue) - current;

                // It's difficult changing jobs, so slightly lower the expected happiness.
                adjustedDiff = diff - 10;
            }

            // TO DO tweak this.
            return Mathf.Max(0, salary + (-adjustedDiff/10 * 1000));
        }
        return baseMinSalary;
    }

    public Stat bestStat {
        get {
            Stat max = charisma;
            if (creativity.value > max.value)
                max = creativity;
            if (cleverness.value > max.value)
                max = cleverness;
            return max;
        }
    }

    // How many weeks the worker is off the job market for.
    // Recent offers the player has made.
    public int offMarketTime;
    public int recentPlayerOffers;

    public Stat happiness;
    public Stat productivity;
    public Stat charisma;
    public Stat creativity;
    public Stat cleverness;

    void Start() {
        Init("Default Worker");
    }
    public Worker Init(string name_) {
        return Init(name_, "Associate", 30000, 0, 0, 0, 0, 0);
    }

    public Worker Init(
            string name_,
            string title_,
            float baseMinSalary_,
            float happiness_,
            float productivity_,
            float charisma_,
            float creativity_,
            float cleverness_ ) {
        name          = name_;
        title         = title_;
        salary        = 0;
        baseMinSalary = baseMinSalary_;
        happiness     = new Stat("Happiness",    happiness_);
        productivity  = new Stat("Productivity", productivity_);
        charisma      = new Stat("Charisma",     charisma_);
        creativity    = new Stat("Creativity",   creativity_);
        cleverness    = new Stat("Cleverness",   cleverness_);

        offMarketTime = 0;
        recentPlayerOffers = 0;

        bio = Worker.BuildBio(this);
        return this;
    }

    public override Stat StatByName(string name) {
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
                return null;
        }
    }

    private static Dictionary<string, Dictionary<string, string[]>> bioMap = new Dictionary<string, Dictionary<string, string[]>> {
        {
            "Creativity", new Dictionary<string, string[]> {
                { "low", new string[] {"is not very imaginative", "is often completely uninspiring"} },
                { "mid", new string[] {"is artistic", "has an eye for trends", "can be quite original"} },
                { "high", new string[] {"is a genius", "is a visionary", "is extremely talented"} }
            }
        },
        {
            "Cleverness", new Dictionary<string, string[]> {
                { "low", new string[] {"is quite dim", "isn't the sharpest tool in the shed", "isn't the brightest bulb of the bunch"} },
                { "mid", new string[] {"is an adept problem solver", "is technically gifted", "solves problems well", "is inventive", "can wear many hats"} },
                { "high", new string[] {"has a beautiful mind", "is absolutely brilliant", "is always on the cutting edge"} }
            }
        },
        {
            "Charisma", new Dictionary<string, string[]> {
                { "low", new string[] {"is alienating", "is hard to be around", "has a weird smell", "is very awkward", "creeps me out"} },
                { "mid", new string[] {"is fun to be around", "can be charming", "is easy to talk to", "is friendly"} },
                { "high", new string[] {"has a magnetic personality", "is hypnotic", "is a natural-born leader", "can be very persuasive", "is capable of rallying people to anything"} }
            }
        },
        {
            "Productivity", new Dictionary<string, string[]> {
                { "low", new string[] {"is kind of lazy", "needs a lot of motivation", "slacks off from time to time"} },
                { "mid", new string[] {"is a diligent worker", "is dedicated", "works hard", "is efficient with time"} },
                { "high", new string[] {"is really 'heads-down'", "multitasks very effectively", "gets a lot done in a short time"} }
            }
        },
        {
            "Happiness", new Dictionary<string, string[]> {
                { "low", new string[] {"is a real bummer", "brings everyone down", "reminds me of Eeyore", "can be very tempermental"} },
                { "mid", new string[] {"brightens up the office", "cheers everyone up", "is very nice"} },
                { "high", new string[] {"would be a great cultural fit", "is a real pleasure to work with", "has an infectious energy"} }
            }
        }
    };

    public static string BuildBio(Worker w) {
        // Randomize order of stats.
        string[] stats = new string[] {"Creativity", "Cleverness", "Charisma", "Productivity", "Happiness"};
        stats = stats.OrderBy(x => Random.value).ToArray();

        float prevVal = -1;
        int maxConj = 3;
        List<string> conjs = new List<string>() {};
        List<string> bio = new List<string> { w.name };

        foreach (string stat in stats) {
            float val = w.StatByName(stat).baseValue;
            string level = SkillLevel(val);

            if (prevVal >= 0) {
                string conj;
                if (System.Math.Abs(val - prevVal) >= 4) {
                    conj = "but";
                } else {
                    conj = "and";
                }

                if (conjs.Count >= maxConj || (conjs.Count > 0 && conjs[conjs.Count - 1] == conj)) {
                    bio[bio.Count - 1] += ".";
                    bio.Add(w.name);
                    conjs.Clear();
                } else {
                    bio.Add(conj);
                    conjs.Add(conj);
                }
            }

            string[] descs = bioMap[stat][level];
            int idx = Random.Range(0,(descs.Length - 1));
            bio.Add(descs[idx]);
            prevVal = val;
        }
        return string.Join(" ", bio.ToArray()) + ".";
    }

    private static string SkillLevel(float val) {
        if (val <= 4)
            return "low";
        if (val <= 8)
            return "mid";
        else
            return "high";
    }


}


