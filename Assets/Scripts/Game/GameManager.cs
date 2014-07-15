using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameManager : Singleton<GameManager> {

    // Disable the constructor so that
    // this must be a singleton.
    protected GameManager() {}

    [HideInInspector]
    public Company playerCompany = new Company("THWON");


    private int weekTime = 15;
    private Month _month = Month.January;
    public string month {
        get { return _month.ToString(); }
    }
    private int _year = 1;
    public int year {
        get { return 2014 + _year; }
    }

    [HideInInspector]
    public int week = 0;

    private enum Month {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public List<IUnlockable> unlocked = new List<IUnlockable>();

    public List<ProductType> unlockedProductTypes {
        get { return unlocked.OfType<ProductType>().ToList(); }
    }
    public List<Industry> unlockedIndustries {
        get { return unlocked.OfType<Industry>().ToList(); }
    }
    public List<Market> unlockedMarkets {
        get { return unlocked.OfType<Market>().ToList(); }
    }
    public List<Worker> unlockedWorkers {
        get { return unlocked.OfType<Worker>().ToList(); }
    }

    // A list of events which could possibly occur.
    private List<GameEvent> candidateEvents = new List<GameEvent>();

    public void NewGame(string companyName) {
        playerCompany = new Company(companyName);
        Application.LoadLevel("Game");
    }

    public void HireWorker(Worker worker) {
        playerCompany.HireWorker(worker);

        // TO DO worker shouldn't be removed from unlockedWorkers
        // but instead from availableWorkers.
        //unlockedWorkers.Remove(worker);
    }

    void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        LoadResources();
    }

    void Start() {

        StartCoroutine(Weekly());
        StartCoroutine(Monthly());
        StartCoroutine(Yearly());

        StartCoroutine(ProductDevelopmentCycle());
        StartCoroutine(ProductRevenueCycle());

        //Debug.Log(gameEvents.Count);
        //Debug.Log(System.Guid.NewGuid());
    }

    void Update() {
    }

    IEnumerator Yearly() {
        int yearTime = weekTime*4*12;
        yield return new WaitForSeconds(yearTime);
        while(true) {
            _year++;
            yield return new WaitForSeconds(yearTime);
        }
    }

    IEnumerator Monthly() {
        int monthTime = weekTime*4;
        yield return new WaitForSeconds(monthTime);
        while(true) {

            if (_month == Month.December) {
                _month = Month.January;
            } else {
                _month++;
            }

            playerCompany.Pay();
            yield return new WaitForSeconds(monthTime);
        }
    }

    IEnumerator Weekly() {
        yield return new WaitForSeconds(weekTime);
        while(true) {
            if (week == 3) {
                week = 0;
            } else {
                week++;
            }
            yield return new WaitForSeconds(weekTime);
        }
    }

    IEnumerator ProductDevelopmentCycle() {
        yield return new WaitForSeconds(weekTime/14);
        while(true) {
            playerCompany.DevelopProducts();

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(weekTime/14 * Random.Range(0.4f, 1.4f));
        }
    }

    IEnumerator ProductRevenueCycle() {
        yield return new WaitForSeconds(weekTime/14);
        while(true) {
            float elapsedTime = weekTime/14 * Random.Range(0.4f, 1.4f);
            playerCompany.HarvestProducts(elapsedTime);

            // Add a bit of randomness to give things
            // a more "natural" feel.
            yield return new WaitForSeconds(elapsedTime);
        }
    }

    public void LoadResources() {
        List<GameEvent> gameEvents = new List<GameEvent>(Resources.LoadAll<GameEvent>("GameEvents"));

        foreach (Worker w in Worker.LoadAll(WorkerType.Employee)) {
            SetUnlocked(w);
        }

        foreach (ProductType pt in ProductType.LoadAll()) {
            SetUnlocked(pt);
        }

        foreach (Industry i in Industry.LoadAll()) {
            SetUnlocked(i);
        }

        foreach (Market m in Market.LoadAll()) {
            SetUnlocked(m);
        }
    }

    private void SetUnlocked(IUnlockable u) {
        if (unlocked.Contains(u)) {
            u.Unlocked = true;
        } else {
            u.Unlocked = false;
        }
    }

    void EnableEvent(GameEvent gameEvent) {
        // Add to candidates.
        candidateEvents.Add(gameEvent);

        // Subscribe to its effect events.
        gameEvent.EventTriggered += OnEvent;
    }

    void DisableEvent(GameEvent gameEvent) {
        if (candidateEvents.Contains(gameEvent)) {
            // Unsubscribe and remove.
            gameEvent.EventTriggered -= OnEvent;
            candidateEvents.Remove(gameEvent);
        }
    }

    void OnEvent(GameEvent e) {
        playerCompany.ApplyBuffs(e.companyEffects);

        foreach (Worker worker in playerCompany.workers) {
            worker.ApplyBuffs(e.workerEffects);
        }

        foreach (ProductEffect pe in e.productEffects) {
            playerCompany.ApplyProductEffect(pe);
        }

        foreach (IUnlockable u in e.unlocks) {
            if (!unlocked.Contains(u)) {
                unlocked.Add(u);
                u.Unlocked = true;
            }
        }
    }
}


