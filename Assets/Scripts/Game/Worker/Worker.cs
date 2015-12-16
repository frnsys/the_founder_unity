/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 * This is the immutable template of a Worker, which is instantiated as AWorker
 */

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Worker : SharedResource<Worker> {
    public static List<AWorker> LoadAll() {
        // Load workers as _copies_ so any changes don't get saved to the actual resources.
        // This does not load special workers, since they are only added to available workers through effects.
        return Resources.LoadAll<Worker>("Workers/Bulk").ToList().Select(w => {
                return new AWorker(w);
        }).ToList();
    }

    public static new Worker Load(string name) {
        Worker w = Resources.Load("Workers/Bulk/" + name) as Worker;
        if (w == null) {
            w = Resources.Load("Founders/" + name) as Worker;
        }
        if (w == null) {
            w = Resources.Load("Founders/Cofounders/" + name) as Worker;
        }
        return w;
    }

    // The worker's avatar in the game world.
    [HideInInspector]
    public GameObject avatar;

    // Each worker has their own style!
    public Material material;

    // Later on, there are robotic workers.
    public bool robot = false;

    public string description;
    public string title;
    public List<string> bio;
    public List<Preference> personalInfo;
    public static float baseLeaveProb = 0.05f;

    public float happiness;
    public float productivity;
    public float charisma;
    public float creativity;
    public float cleverness;
    public float baseMinSalary;

    // WORKER BIO GENERATION ---------------------------

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

    public static List<string> BuildBio(Worker w) {
        // Randomize order of stats.
        string[] stats = new string[] {"Creativity", "Cleverness", "Charisma", "Productivity", "Happiness"};
        stats = stats.OrderBy(x => UnityEngine.Random.value).ToArray();

        List<string> bio = new List<string>();

        foreach (string stat in stats) {
            float val = w.StatByName(stat);
            string level = SkillLevel(val);

            string[] descs = bioMap[stat][level];
            int idx = UnityEngine.Random.Range(0,(descs.Length - 1));
            bio.Add(descs[idx]);
        }
        return bio;
    }

    private static string SkillLevel(float val) {
        if (val <= 4)
            return "low";
        if (val <= 8)
            return "mid";
        else
            return "high";
    }

    // There are certain preferences for which
    // employees are willing to take less salary
    // if mentioned in conversation
    public enum Preference {
        HEALTHCARE,
        VACATION,
        CULTURE,
        LOANS, // student loans
        DEBT, // credit card debt
        RETIREMENT,
        PARENTAL,
        FITNESS,
        NONE
    }

    public static List<Preference> BuildPreferences(Worker w) {
        List<Preference> prefs = new List<Preference>();
        foreach (Preference p in Enum.GetValues(typeof(Preference)).Cast<Preference>()) {
            if (p != Preference.NONE && UnityEngine.Random.value < 0.25) {
                prefs.Add(p);
            }
        }
        return prefs;
    }

    public static Dictionary<Preference, string[]> prefToDescMap = new Dictionary<Preference, string[]> {
        {Preference.HEALTHCARE, new string[] {"has a sick relative", "is a hypochondriac", "has a chronic illness"}},
        {Preference.VACATION, new string[] {"values time off", "likes to travel", "enjoys rest and relaxation"}},
        {Preference.CULTURE, new string[] {"prefers a fun environment", "enjoys joking with coworkers", "likes an social workplace"}},
        {Preference.LOANS, new string[] {"has student loans", "went to an expensive school", "had a job through college"}},
        {Preference.DEBT, new string[] {"shops online a lot", "has expensive tastes", "spends a lot of money"}},
        {Preference.RETIREMENT, new string[] {"worried about financial security", "concerned about growing old", "anxious about social security"}},
        {Preference.PARENTAL, new string[] {"wants to have kids", "has children", "planning a family"}},
        {Preference.FITNESS, new string[] {"values exercise", "anxious about body image", "wants to stay fit"}}
    };

    public static Dictionary<string, Dictionary<Preference, string[]>> dialogueToDialogueMap = new Dictionary<string, Dictionary<Preference, string[]>> {
        {"Been anywhere fun lately?", new Dictionary<Preference, string[]> {
            {Preference.DEBT, new string[] {"Haven't been able to afford it"}},
            {Preference.LOANS, new string[] {"Haven't been able to afford it"}},
            {Preference.VACATION, new string[] {"Yes, I love to travel!", "We just got back from a great trip!"}},
            {Preference.FITNESS, new string[] {"I went on a long bike ride out of town"}},
            {Preference.NONE, new string[] {"Not really"}}
        }},
        {"What are you up to this weekend?", new Dictionary<Preference, string[]> {
            {Preference.PARENTAL, new string[] {"Taking our kids out to a show.", "Watching my kid's soccer game.", "Preparing our baby shower!"}},
            {Preference.VACATION, new string[] {"Taking a trip to see some sights.", "Heading out to the beach!"}},
            {Preference.CULTURE, new string[] {"Grabbing drinks with some former coworkers", "Catching up with some old colleagues"}},
            {Preference.FITNESS, new string[] {"Running a 5k", "Going for a long bike ride"}},
            {Preference.NONE, new string[] {"Not much"}}
        }},
        {"What do you like to do outside of work?", new Dictionary<Preference, string[]> {
            {Preference.PARENTAL, new string[] {"Don't have much free time, the kids are a handful"}},
            {Preference.VACATION, new string[] {"I love to travel", "I love visiting new places"}},
            {Preference.FITNESS, new string[] {"I enjoy running", "I like to bike a lot"}},
            {Preference.NONE, new string[] {"Not much"}}
        }},
        {"Where do you see yourself in 5 years?", new Dictionary<Preference, string[]> {
            {Preference.RETIREMENT, new string[] {"Hoping I can retire by then!"}},
            {Preference.VACATION, new string[] {"I'd love to visit Europe at some point", "I'm planning on taking a trip through Asia before long"}},
            {Preference.FITNESS, new string[] {"Hoping that I'll have run a full marathon!"}},
            {Preference.PARENTAL, new string[] {"Seeing my kid graduate from a prestigious university"}},
            {Preference.CULTURE, new string[] {"Taking part in a vibrant and exciting workplace", "Participating in a thriving company"}},
            {Preference.NONE, new string[] {"I dunno"}}
        }},
    };

    public static Dictionary<Preference, string> prefToDialogueMap = new Dictionary<Preference, string> {
        {Preference.HEALTHCARE, "We have some great healthcare benefits included"},
        {Preference.VACATION, "We have a great vacation policy"},
        {Preference.CULTURE, "We really value our workplace culture"},
        {Preference.LOANS, "We provide student loan forgiveness programs"},
        {Preference.DEBT, "We often help employees with debt management"},
        {Preference.RETIREMENT, "We provide a generous 401k plan"},
        {Preference.PARENTAL, "We have great programs and services to help with childcare"},
        {Preference.FITNESS, "We have a lot of great fitness benefits"}
    };

    public static Dictionary<Preference, float> prefToDiscountMap = new Dictionary<Preference, float> {
        {Preference.HEALTHCARE, 0.8f},
        {Preference.VACATION, 0.85f},
        {Preference.CULTURE, 0.9f},
        {Preference.LOANS, 0.75f},
        {Preference.DEBT, 0.75f},
        {Preference.RETIREMENT, 0.9f},
        {Preference.PARENTAL, 0.85f},
        {Preference.FITNESS, 0.9f}
    };

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
                return 0f;
        }
    }
}


