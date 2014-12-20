/*
 * The central manager for everything in the game.
 * Delegates to other managers for more specific domains.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {

    // Disable the constructor so that
    // this must be a singleton.
    protected GameManager() {}

    // All the game data which gets persisted.
    private GameData data;

    public Company playerCompany {
        get { return data.company; }
    }
    public UnlockSet unlocked {
        get { return data.unlocked; }
    }

    public float economyMultiplier {
        get { return data.economyMultiplier; }
    }

    // Other managers.
    [HideInInspector]
    public ResearchManager researchManager;

    [HideInInspector]
    public NarrativeManager narrativeManager;

    [HideInInspector]
    public GameConfig config;

    public List<Worker> availableWorkers {
        get {
            return data.unlocked.workers.Where(w => !playerCompany.workers.Contains(w)).ToList();
        }
    }

    // Load existing game data.
    public void Load(GameData d) {
        researchManager = gameObject.AddComponent<ResearchManager>();
        narrativeManager = gameObject.AddComponent<NarrativeManager>();

        data = d;
        researchManager.Load(d);
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);

        // Load internal config.
        config = Resources.Load("GameConfig") as GameConfig;

        if (data == null) {
            Load(GameData.New("DEFAULTCORP"));
        }
    }

    void OnEnable() {
        GameEvent.EventTriggered += OnEvent;
        ResearchManager.Completed += OnResearchCompleted;
        Product.Completed += OnProductCompleted;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        ResearchManager.Completed -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
    }

    void Start() {
        StartCoroutine(Weekly());
        StartCoroutine(Monthly());
        StartCoroutine(Yearly());

        StartCoroutine(ProductDevelopmentCycle());
        StartCoroutine(ProductRevenueCycle());
        StartCoroutine(ResearchCycle());

        // TESTing hello from your mentor!
        //narrativeManager.MentorMessage("A message from your mentor", "Welcome to The Founder!");
    }

    void OnEvent(GameEvent e) {
        ApplyEffectSet(e.effects);

        // If this event is not repeatable,
        // remove it from the candidate event pool.
        if (!e.repeatable) {
            data.unlocked.events.Remove(e);
        }
    }

    void OnResearchCompleted(Technology t) {
        ApplyEffectSet(t.effects);
    }

    void OnProductCompleted(Product p) {
        ApplyEffectSet(p.effects);
    }

    public void ApplyEffectSet(EffectSet es) {
        playerCompany.ApplyEffectSet(es);
        data.unlocked.Unlock(es.unlocks);
    }

    // ===============================================
    // Time ==========================================
    // ===============================================

    private int weekTime = 15;
    public string month {
        get { return data.month.ToString(); }
    }
    public int year {
        get { return 2014 + data.year; }
    }
    [HideInInspector]
    public int week {
        get { return data.week; }
    }



    public void Pause() {
        Time.timeScale = 0;
    }
    public void Resume() {
        Time.timeScale = 1;
    }

    IEnumerator Yearly() {
        int yearTime = weekTime*4*12;
        yield return new WaitForSeconds(yearTime);
        while(true) {
            data.year++;

            // Get the annual performance data and generate the report.
            List<PerformanceDict> annualData = playerCompany.CollectAnnualPerformanceData();
            PerformanceDict results = annualData[0];
            PerformanceDict deltas = annualData[1];
            data.board.EvaluatePerformance(deltas);

            UIManager.Instance.AnnualReport(results, deltas, data.board);

            // Lose condition:
            if (data.board.happiness < -20)
                // TO DO this should be a proper "lose game"
                UIManager.Instance.Alert("YOU LOSE");

            yield return new WaitForSeconds(yearTime);
        }
    }

    IEnumerator Monthly() {
        int monthTime = weekTime*4;
        yield return new WaitForSeconds(monthTime);
        while(true) {

            if (data.month == Month.December) {
                data.month = Month.January;
            } else {
                data.month++;
            }

            // AI companies gather business intelligence!!
            foreach (AICompany aic in data.otherCompanies) {
                aic.CollectPerformanceData();
                aic.PayMonthly();
            }

            // See how the economy is.
            data.economyMultiplier = EconomyMultiplier();

            playerCompany.CollectPerformanceData();
            playerCompany.PayMonthly();
            yield return new WaitForSeconds(monthTime);
        }
    }

    IEnumerator Weekly() {
        yield return new WaitForSeconds(weekTime);
        while(true) {
            if (data.week == 3) {
                data.week = 0;
            } else {
                data.week++;
            }

            // TO DO this should be a proper "lose game"
            // Do you die?
            if (data.year > data.lifetimeYear &&
                (int)data.month > data.lifetimeMonth &&
                data.week > data.lifetimeWeek) {
                UIManager.Instance.Alert("YOU DIE YOUR EMPIRE IS IN RUINS");
            }

            foreach (AICompany aic in data.otherCompanies) {
                aic.Decide();
            }

            // Save the game every week.
            //GameData.Save(data);

            yield return new WaitForSeconds(weekTime);
        }
    }

    IEnumerator ProductDevelopmentCycle() {
        yield return new WaitForSeconds(weekTime/14);
        while(true) {
            playerCompany.DevelopProducts();

            foreach (AICompany aic in data.otherCompanies) {
                aic.DevelopProducts();
            }

            // TO DO Temporarily placed here
            GameEvent.Roll(data.unlocked.events);

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(weekTime/14 * Random.Range(0.4f, 1.4f));
        }
    }

    IEnumerator ProductRevenueCycle() {
        yield return new WaitForSeconds(weekTime/14);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = weekTime/14 * Random.Range(0.4f, 1.4f);
            playerCompany.HarvestProducts(elapsedTime);

            foreach (AICompany aic in data.otherCompanies) {
                aic.HarvestProducts(elapsedTime);
            }

            yield return new WaitForSeconds(elapsedTime);
        }
    }

    IEnumerator ResearchCycle() {
        yield return new WaitForSeconds(weekTime/14);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = weekTime/14 * Random.Range(0.4f, 1.4f);
            researchManager.Research();

            yield return new WaitForSeconds(elapsedTime);
        }
    }

    // http://stackoverflow.com/q/5817490/1097920
    public static double RandomGaussian()
    {
        double U, u, v, S;

        do
        {
            u = 2.0 * Random.value - 1.0;
            v = 2.0 * Random.value - 1.0;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        double fac = System.Math.Sqrt(-2.0 * System.Math.Log(S) / S);
        return u * fac;
    }

    public float EconomyMultiplier() {
        float r = (float)RandomGaussian();

        float mean = 1;
        float std  = 0.2f;
        return (r * std) + 1;
    }

}


