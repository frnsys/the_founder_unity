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
        // Clean up existing managers, if any.
        if (researchManager != null)
            Destroy(researchManager);
        if (narrativeManager != null)
            Destroy(narrativeManager);
        if (workerManager != null)
            Destroy(workerManager);
        if (eventManager != null)
            Destroy(eventManager);

        researchManager  = gameObject.AddComponent<ResearchManager>();
        narrativeManager = gameObject.AddComponent<NarrativeManager>();
        workerManager    = gameObject.AddComponent<WorkerManager>();
        eventManager     = gameObject.AddComponent<EventManager>();

        data = d;
        eventManager.Load(d);
        workerManager.Load(d);
        researchManager.Load(d);
        narrativeManager.Load(d);
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);

        // Load internal config.
        config = Resources.Load("GameConfig") as GameConfig;

        if (data == null) {
            Load(GameData.New("DEFAULTCORP"));
        }
    }

    void OnLevelWasLoaded(int level) {
        // See Build Settings to get the number for levels/scenes.
        if (level == 2) {
            StartGame();
            narrativeManager.InitializeOnboarding();
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
        // Uncomment this to start a game directly (i.e. skipping the new game/cofounder selection).
        //StartGame();

        // Uncomment this if you want to start the game with onboarding.
        // narrativeManager.InitializeOnboarding();
    }

    void StartGame() {
        StartCoroutine(Weekly());
        StartCoroutine(Monthly());
        StartCoroutine(Yearly());

        StartCoroutine(ProductDevelopmentCycle());
        StartCoroutine(ProductRevenueCycle());
        StartCoroutine(ResearchCycle());
        StartCoroutine(OpinionCycle());
        StartCoroutine(EventCycle());

        // We only load this here because the UIOfficeViewManager
        // doesn't exist until the game starts.
        UIOfficeViewManager.Instance.Load(data);
    }

    void OnEvent(GameEvent e) {
        ApplyEffectSet(e.effects);
    }

    void OnResearchCompleted(Technology t) {
        ApplyEffectSet(t.effects);
    }

    void OnProductCompleted(Product p, Company c) {
        // TO DO this should apply to the AI company as well.
        if (c == data.company)
            ApplyEffectSet(p.effects);
    }

    public void ApplyEffectSet(EffectSet es) {
        es.Apply(playerCompany);
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

    static public event System.Action<int> YearEnded;
    IEnumerator Yearly() {
        int yearTime = weekTime*4*12;
        yield return new WaitForSeconds(yearTime);
        while(true) {
            data.year++;

            if (YearEnded != null)
                YearEnded(data.year);

            yield return new WaitForSeconds(yearTime);
        }
    }

    static public event System.Action<int, PerformanceDict, PerformanceDict, TheBoard> PerformanceReport;
    static public event System.Action<Company> GameLost;
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

            if ((int)data.month % 4 == 0) {
                // Get the quarterly performance data and generate the report.
                List<PerformanceDict> quarterData = playerCompany.CollectQuarterlyPerformanceData();
                PerformanceDict results = quarterData[0];
                PerformanceDict deltas = quarterData[1];
                data.board.EvaluatePerformance(deltas);

                if (PerformanceReport != null) {
                    int quarter = (int)data.month/4 + 1;
                    PerformanceReport(quarter, results, deltas, data.board);
                }

                // Lose condition:
                if (data.board.happiness < -20)
                    GameLost(playerCompany);
            }

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

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(cycleTime * Random.Range(0.4f, 1.4f));
        }
    }

    IEnumerator EventCycle() {
        yield return new WaitForSeconds(weekTime);
        while(true) {
            eventManager.Tick();
            eventManager.EvaluateSpecialEvents();

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(weekTime * Random.Range(0.9f, 1.6f));
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
