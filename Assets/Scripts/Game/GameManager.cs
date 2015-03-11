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
    public Infrastructure infrastructureCostMultiplier {
        get { return data.infrastructureCostMultiplier; }
        set { data.infrastructureCostMultiplier = value; }
    }

    public float profitTarget {
        get { return data.board.profitTarget; }
    }

    public bool workerInsight {
        get { return data.workerInsight; }
    }
    public bool cloneable {
        get { return data.cloneable; }
    }

    public List<AICompany> activeAICompanies {
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
        foreach (AICompany aic in AICompany.all) {
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
        Product.Completed += OnProductCompleted;
        SpecialProject.Completed += OnSpecialProjectCompleted;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        Company.ResearchCompleted -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
        SpecialProject.Completed -= OnSpecialProjectCompleted;
    }

    void OnLevelWasLoaded(int level) {
        // See Build Settings to get the number for levels/scenes.
        if (Application.loadedLevelName == "Game") {
            StartGame();
            narrativeManager.InitializeOnboarding();
        }
    }

    void Start() {
#if UNITY_EDITOR
        // TESTING start a test game.
        if (Application.loadedLevelName == "Game") {
            Founder cofounder = Resources.LoadAll<Founder>("Founders/Cofounders").First();
            Location location = Location.Load("San Francisco");
            Vertical vertical = Vertical.Load("Information");
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
    }

    public void InitializeGame(Founder cofounder, Location location, Vertical vertical) {
        data.company.verticals = new List<Vertical> { vertical };

        if (vertical.name == "Information") {
            data.company.infrastructure[Infrastructure.Type.Datacenter] = 6;
        } else {
            data.company.infrastructure[Infrastructure.Type.Factory] = 6;
        }

        data.company.SetHQ(location);
        data.unlocked.locations = new List<Location> { location };

        data.company.founders.Add(cofounder);
        ApplyEffectSet(cofounder.bonuses);

        // The cofounders you didn't pick start their own rival company.
        AICompany aic = AICompany.Find("Rival Corp");
        List<Founder> cofounders = Resources.LoadAll<Founder>("Founders/Cofounders").Where(c => c != cofounder).ToList();
        aic.founders = cofounders;
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

        // We only load this here because the UIOfficeManager
        // doesn't exist until the game starts.
        UIOfficeManager.Instance.Load(data);
    }

    void OnEvent(GameEvent e) {
        if (e.effects != null)
            ApplyEffectSet(e.effects);
    }

    void OnResearchCompleted(Technology t) {
        ApplyEffectSet(t.effects);
    }

    public void OnProductCompleted(Product p, Company c) {
        if (c == data.company) {
            // If a product of this type combo already exists,
            // do not re-apply the effects.
            if(c.products.Count(p_ => p_.comboID == p.comboID) == 1)
                ApplyEffectSet(p.effects);
        }
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
            case EffectSet.Special.FounderAI:
                narrativeManager.GameWon();
                break;
        }
    }

    // ===============================================
    // Time ==========================================
    // ===============================================

    // In seconds
    private static int weekTime = 6;
    private static float cycleTime = weekTime/12f;
    public static float CycleTime {
        get { return cycleTime; }
    }
    public string month {
        get { return Month.GetName(typeof(Month), data.month); }
    }
    public int numMonth {
        get { return (int)data.month + 1; }
    }
    public int year {
        get { return 2000 + data.year; }
    }
    [HideInInspector]
    public int week {
        get { return data.week; }
    }
    public int date {
        get { return int.Parse(string.Format("{0}{1:00}{2}", year, numMonth, week)); }
    }

    public void Pause() {
        pauses++;
        Time.timeScale = 0;
        Debug.Log(pauses);
    }
    public void Resume() {
        pauses--;
        if (pauses == 0)
            Time.timeScale = 1;
        Debug.Log(pauses);
    }
    // So we can keep track of how many pauses have been called.
    public int pauses;

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
    IEnumerator Monthly() {
        int monthTime = weekTime*4;
        yield return new WaitForSeconds(monthTime);
        while(true) {
            if (data.month == Month.December) {
                data.month = Month.January;
            } else {
                data.month++;
            }

            playerCompany.PayMonthly();

            // You only need to start making profit after the first year.
            if ((int)data.month % 3 == 0 && (int)data.year > 2) {
                // Get the annual performance data and generate the report.
                List<PerformanceDict> annualData = playerCompany.CollectAnnualPerformanceData();
                PerformanceDict results = annualData[0];
                PerformanceDict deltas = annualData[1];
                float growth = data.board.EvaluatePerformance(results["Annual Profit"]);

                if (PerformanceReport != null) {
                    PerformanceReport(data.year, results, deltas, data.board);
                }

                // Schedule a news story about the growth (if it warrants one).
                StartCoroutine(PerformanceNews(growth));

                // Lose condition:
                // TEMP disabled
                //if (data.board.happiness < -20)
                    //narrativeManager.GameLost();
            }

            yield return new WaitForSeconds(monthTime);
        }
    }

    IEnumerator PerformanceNews(float growth) {
        yield return new WaitForSeconds(weekTime);
        GameEvent ev = null;
        float target = data.board.desiredGrowth;
        if (growth >= target * 2) {
            ev = GameEvent.LoadSpecialEvent("Faster Growth");
        } else if (growth >= target * 1.2) {
            ev = GameEvent.LoadSpecialEvent("Fast Growth");
        } else if (growth <= target * 0.8) {
            ev = GameEvent.LoadSpecialEvent("Slow Growth");
        } else if (growth <= target * 0.6) {
            ev = GameEvent.LoadSpecialEvent("Slower Growth");
        }
        GameEvent.Trigger(ev);
    }

    IEnumerator Weekly() {
        yield return new WaitForSeconds(weekTime);
        while(true) {
            if (data.week == 3) {
                data.week = 0;
            } else {
                data.week++;
            }

            if (data.year > data.lifetimeYear &&
                (int)data.month > data.lifetimeMonth &&
                data.week > data.lifetimeWeek) {

                if (!data.immortal) {
                    GameEvent ev = GameEvent.LoadNoticeEvent("Death");
                    GameEvent.Trigger(ev);

                    // Pay inheritance tax.
                    float tax = playerCompany.cash.value * 0.75f;
                    playerCompany.Pay(tax);
                    UIManager.Instance.SendPing(string.Format("Paid {0:C0} in inheritance taxes.", tax), Color.red);
                } else {
                    GameEvent ev = GameEvent.LoadNoticeEvent("Immortal");
                    GameEvent.Trigger(ev);
                }
            }

            // A random AI company makes a move.
            if (Random.value < 0.02)
                activeAICompanies[Random.Range(0, activeAICompanies.Count)].Decide();

            // Update workers' off market times.
            foreach (Worker w in workerManager.AllWorkers.Where(w => w.offMarketTime > 0)) {
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
            playerCompany.DevelopProduct();

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

            playerCompany.HarvestProducts(elapsedTime);
            playerCompany.HarvestCompanies();

            yield return new WaitForSeconds(elapsedTime);
        }
    }

    IEnumerator ResearchCycle() {
        yield return new WaitForSeconds(cycleTime);
        while(true) {
            // Add a bit of randomness to give things
            // a more "natural" feel.
            float elapsedTime = cycleTime * Random.Range(0.4f, 1.4f);
            playerCompany.Research();

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
            // Deflate hype.
            playerCompany.ForgetOpinionEvents();
            playerCompany.publicity.baseValue = Mathf.Max(0, playerCompany.publicity.baseValue - 0.1f);

            yield return new WaitForSeconds(elapsedTime);
        }
    }

}
