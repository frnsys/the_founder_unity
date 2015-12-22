/*
 * The central manager for everything in the game.
 * Delegates to other managers for more specific domains.
 */

using UnityEngine;
using System.Linq;
using System.Threading;
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
    public List<AAICompany> otherCompanies {
        get { return data.otherCompanies; }
    }
    public UnlockSet unlocked {
        get { return data.unlocked; }
    }

    // "World" variables.
    public Economy economy {
        get { return data.economy; }
    }
    public float economyMultiplier {
        get { return economyManager.economyMultiplier; }
    }
    public float economicStability {
        get { return data.economicStability; }
        set { data.economicStability = value; }
    }
    public float spendingMultiplier {
        get { return data.spendingMultiplier; }
        set { data.spendingMultiplier = value; }
    }
    public float wageMultiplier {
        get { return data.wageMultiplier; }
        set { data.wageMultiplier = value; }
    }
    public float forgettingRate {
        get { return data.forgettingRate; }
        set { data.forgettingRate = value; }
    }
    public float taxRate {
        get { return data.taxRate; }
        set { data.taxRate = value; }
    }
    public float expansionCostMultiplier {
        get { return data.expansionCostMultiplier; }
        set { data.expansionCostMultiplier = value; }
    }
    public float costMultiplier {
        get { return data.costMultiplier; }
        set { data.costMultiplier = value; }
    }

    public float profitTarget {
        get { return data.board.profitTarget; }
    }

    public TheBoard.Status boardStatus {
        get { return data.board.status; }
    }

    public bool workerInsight {
        get { return data.workerInsight; }
    }
    public bool workerQuant {
        get { return data.workerQuant; }
    }
    public bool cloneable {
        get { return data.cloneable; }
    }
    public bool automation {
        get { return data.automation; }
    }

    public List<AAICompany> activeAICompanies {
        get { return data.otherCompanies.FindAll(a => !a.disabled); }
    }

    // Other managers.
    [HideInInspector]
    public NarrativeManager narrativeManager;

    [HideInInspector]
    public EconomyManager economyManager;

    [HideInInspector]
    public WorkerManager workerManager;

    [HideInInspector]
    public EventManager eventManager;

    // Load existing game data.
    public void Load(GameData d) {
        // Clean up existing managers, if any.
        if (narrativeManager != null)
            Destroy(narrativeManager);
        if (workerManager != null)
            Destroy(workerManager);
        if (eventManager != null)
            Destroy(eventManager);
        if (economyManager != null)
            Destroy(economyManager);

        narrativeManager = gameObject.AddComponent<NarrativeManager>();
        workerManager    = gameObject.AddComponent<WorkerManager>();
        eventManager     = gameObject.AddComponent<EventManager>();
        economyManager   = gameObject.AddComponent<EconomyManager>();

        data = d;
        eventManager.Load(d);
        workerManager.Load(d);
        economyManager.Load(d);
        narrativeManager.Load(d);

        // Setup all the AI companies.
        AICompany.all = data.otherCompanies;
        foreach (AAICompany aic in AICompany.all) {
            aic.Setup();
        }
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);

        if (data == null) {
            Load(GameData.New("DEFAULTCORP"));
        }
    }

    void OnEnable() {
        GameEvent.EventTriggered += OnEvent;
        Company.ResearchCompleted += OnResearchCompleted;
        SpecialProject.Completed += OnSpecialProjectCompleted;
        MainGame.Done += OnGameDone;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        Company.ResearchCompleted -= OnResearchCompleted;
        SpecialProject.Completed -= OnSpecialProjectCompleted;
        MainGame.Done -= OnGameDone;
    }

    void OnLevelWasLoaded(int level) {
        // See Build Settings to get the number for levels/scenes.
        if (Application.loadedLevelName == "Game") {
            StartGame();
            Debug.Log(data.obs);
            narrativeManager.InitializeOnboarding();
        }
    }

    void Start() {
#if UNITY_EDITOR
        // TESTING start a test game.
        if (Application.loadedLevelName == "Game") {
            Worker cofounder = Resources.LoadAll<Worker>("Founders/Cofounders").First();
            Location location = Location.Load("San Francisco");
            Vertical vertical = Vertical.Load("Hardware");
            InitializeGame(cofounder, location, vertical);
            StartGame();

            // Uncomment this if you want to start the game with onboarding.
            //narrativeManager.InitializeOnboarding();
        }
#endif

        // So negative currency values are shown as -$1000 instead of ($1000).
        System.Globalization.CultureInfo modCulture = new System.Globalization.CultureInfo("en-US");
        modCulture.NumberFormat.CurrencyNegativePattern = 1;
        Thread.CurrentThread.CurrentCulture = modCulture;

        // Limit the FPS for better battery life.
        Application.targetFrameRate = 30;
    }

    public void InitializeGame(Worker cofounder, Location location, Vertical vertical) {
        // TESTING
        Vertical vertical2 = Vertical.Load("Information");
        data.company.verticals = new List<Vertical> { vertical, vertical2 };
        //AGameEvent ge = GameEvent.LoadNoticeEvent("testevent");
        //ge.countdown = 2;
        //ge.probability = 1f;
        //eventManager.Add(ge);


        foreach (ProductType pt in ProductType.LoadAll().Where(p => p.isAvailable(data.company))) {
            data.company.productTypes.Add(pt);
        }

        data.company.SetHQ(location);
        data.company.founders.Add(new AWorker(cofounder));

        if (cofounder.name == "Mark Stanley") {
            data.company.cash.baseValue += 50000;
        }

        // The cofounders you didn't pick start their own rival company.
        AAICompany aic = AICompany.Find("Rival Corp");
        List<Worker> cofounders = Resources.LoadAll<Worker>("Founders/Cofounders").Where(c => c != cofounder).ToList();
        aic.founder = cofounders[0];
    }

    void StartGame() {
        StartCoroutine(GameTimer.Start());

        // We only load this here because the UIOfficeManager
        // doesn't exist until the game starts.
        UIOfficeManager.Instance.Load(data);
    }

    public void SaveGame() {
        GameData.Save(data);
    }

    void OnEvent(GameEvent e) {
        if (e != null && e.effects != null)
            ApplyEffectSet(e.effects);
    }

    void OnResearchCompleted(Technology t) {
        ApplyEffectSet(t.effects);
    }

    public void OnSpecialProjectCompleted(SpecialProject p) {
        ApplyEffectSet(p.effects);
    }

    public void ApplyEffectSet(EffectSet es) {
        es.Apply(playerCompany);
        data.unlocked.Unlock(es.unlocks);
    }

    public void ApplySpecialEffect(EffectSet.Special e) {
        switch(e) {
            case EffectSet.Special.Immortal:
                data.immortal = true;
                break;
            case EffectSet.Special.Cloneable:
                data.cloneable = true;
                break;
            case EffectSet.Special.Prescient:
                data.prescient = true;
                break;
            case EffectSet.Special.WorkerInsight:
                data.workerInsight = true;
                break;
            case EffectSet.Special.WorkerQuant:
                data.workerQuant = true;
                break;
            case EffectSet.Special.Automation:
                data.automation = true;
                break;
            case EffectSet.Special.FounderAI:
                narrativeManager.GameWon();
                break;
        }
    }

    public int year {
        get { return 2000 + data.year; }
    }
    public int age {
        get { return 25 + data.year; }
    }
    public int companyAge {
        get { return data.year; }
    }

    public void Pause() {
        GameTimer.Pause();
    }
    public void Resume() {
        GameTimer.Resume();
    }

    static public event System.Action<int> YearEnded;
    static public event System.Action<int, Company.StatusReport, TheBoard> PerformanceReport;
    //IEnumerator PerformanceNews(float growth) {
        //yield return StartCoroutine(GameTimer.Wait(weekTime));
        //AGameEvent ev = null;
        //float target = data.board.desiredGrowth;
        //if (growth >= target * 2) {
            //ev = GameEvent.LoadNoticeEvent("Faster Growth");
        //} else if (growth >= target * 1.2) {
            //ev = GameEvent.LoadNoticeEvent("Fast Growth");
        //} else if (growth <= target * 0.8) {
            //ev = GameEvent.LoadNoticeEvent("Slow Growth");
        //} else if (growth <= target * 0.6) {
            //ev = GameEvent.LoadNoticeEvent("Slower Growth");
        //}
        //if (ev != null)
            //GameEvent.Trigger(ev.gameEvent);
    //}

    public void OnGameDone() {
        data.year++;

        // Harvest minicompanies (acquisitions)
        playerCompany.HarvestCompanies();

        // Check performance and update profit target
        Company.StatusReport report = playerCompany.CollectAnnualPerformanceData();
        float growth = data.board.EvaluatePerformance(report.profit);

        // Show performance report
        if (PerformanceReport != null)
            PerformanceReport(data.year, report, data.board);

        if (YearEnded != null)
            YearEnded(data.year);

        SaveGame();
    }

}
