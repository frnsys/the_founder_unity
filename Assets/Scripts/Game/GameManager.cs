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
    public List<Company> allCompanies {
        get {
            List<Company> allCos = new List<Company>(data.otherCompanies.Cast<Company>());
            allCos.Add(playerCompany);
            return allCos;
        }
    }
    public UnlockSet unlocked {
        get { return data.unlocked; }
    }

    public float economyMultiplier {
        get { return data.economyMultiplier; }
    }

    public int maxProductTypes {
        get { return data.maxProductTypes; }
    }

    public WorkerInsight workerInsight {
        get { return data.workerInsight; }
    }

    // Other managers.
    [HideInInspector]
    public ResearchManager researchManager;

    [HideInInspector]
    public NarrativeManager narrativeManager;

    [HideInInspector]
    public WorkerManager workerManager;

    [HideInInspector]
    public EventManager eventManager;

    [HideInInspector]
    public GameConfig config;

    // Load existing game data.
    public void Load(GameData d) {
        researchManager  = gameObject.AddComponent<ResearchManager>();
        narrativeManager = gameObject.AddComponent<NarrativeManager>();
        workerManager    = gameObject.AddComponent<WorkerManager>();
        eventManager     = gameObject.AddComponent<EventManager>();

        data = d;
        researchManager.Load(d);
        eventManager.Load(d);
        workerManager.Load(d);
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
        StartCoroutine(OpinionCycle());

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

    private static int weekTime = 15;
    private static float cycleTime = weekTime/14;
    public static float CycleTime {
        get { return cycleTime; }
    }
    public string month {
        get { return data.month.ToString(); }
    }
    public int year {
        get { return 2000 + data.year; }
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

            // Anniversary/birthday alert!
            int age = 25 + data.year;
            int lastDigit = age % 10;
            string ending = "th";
            if (lastDigit == 1)
                ending = "st";
            else if (lastDigit == 2)
                ending = "nd";
            else if (lastDigit == 3)
                ending = "rd";
            UIManager.Instance.Alert("Happy " + data.year + ending + " birthday!");

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
            data.economyMultiplier = Utils.RandomGaussian(1f, 0.2f);

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

            // Make other AI company moves.
            foreach (AICompany aic in data.otherCompanies) {
                aic.Decide();
            }

            // Update workers' off market times.
            foreach (Worker w in data.unlocked.workers.Where(w => w.offMarketTime > 0)) {
                // Reset player offers if appropriate.
                if (--w.offMarketTime == 0) {
                    w.recentPlayerOffers = 0;
                }
            }

            // Save the game every week.
            //GameData.Save(data);

            yield return new WaitForSeconds(weekTime);
        }
    }

    IEnumerator ProductDevelopmentCycle() {
        yield return new WaitForSeconds(cycleTime);
        while(true) {
            playerCompany.DevelopProducts();

            foreach (AICompany aic in data.otherCompanies) {
                aic.DevelopProducts();
            }

            // TO DO Temporarily placed here
            GameEvent.Roll(data.unlocked.events);

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(cycleTime * Random.Range(0.4f, 1.4f));
        }
    }

    IEnumerator ProductRevenueCycle() {
        yield return new WaitForSeconds(cycleTime);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = cycleTime * Random.Range(0.4f, 1.4f);

            MarketManager.CalculateMarketShares(allCompanies);

            playerCompany.HarvestProducts(elapsedTime);

            foreach (AICompany aic in data.otherCompanies) {
                aic.HarvestProducts(elapsedTime);
            }

            yield return new WaitForSeconds(elapsedTime);
        }
    }

    IEnumerator ResearchCycle() {
        yield return new WaitForSeconds(cycleTime);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = cycleTime * Random.Range(0.4f, 1.4f);
            researchManager.Research();

            yield return new WaitForSeconds(elapsedTime);
        }
    }

    IEnumerator OpinionCycle() {
        yield return new WaitForSeconds(cycleTime);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = cycleTime * Random.Range(0.4f, 1.4f);

            // Pull back opinion effects towards 0.
            // Advance promos.
            playerCompany.ForgetOpinionEvents();
            playerCompany.DevelopPromo();
            foreach (AICompany aic in data.otherCompanies) {
                aic.ForgetOpinionEvents();
                aic.DevelopPromo();
            }

            yield return new WaitForSeconds(elapsedTime);
        }
    }

}
