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
    public Company playerCompany;

    // Other managers.
    public ResearchManager researchManager;

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

    private enum Phase {
        Local,
        Global,
        Planetary,
        Galactic
    }
    private Phase phase = Phase.Local;

    public UnlockSet unlocked = new UnlockSet();
    public List<Worker> availableWorkers {
        get {
            return unlocked.workers.Where(w => !playerCompany.workers.Contains(w)).ToList();
        }
    }

    public void NewGame(string companyName) {
        playerCompany = new Company(companyName);
        Application.LoadLevel("Game");
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);

        if (playerCompany == null) {
            playerCompany = new Company("Foobar Inc");
        }
    }

    void OnEnable() {
        GameEvent.EventTriggered += OnEvent;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
    }

    void Start() {
        StartCoroutine(Weekly());
        StartCoroutine(Monthly());
        StartCoroutine(Yearly());

        StartCoroutine(ProductDevelopmentCycle());
        StartCoroutine(ProductRevenueCycle());
        StartCoroutine(ResearchCycle());

        researchManager = gameObject.AddComponent<ResearchManager>();
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

            playerCompany.PayMonthly();
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

            // Temporarily placed here
            GameEvent.Roll(unlocked.events);

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

    public void Pause() {
        Time.timeScale = 0;
    }
    public void Resume() {
        Time.timeScale = 1;
    }


    void OnEvent(GameEvent e) {
        ApplyEffectSet(e.effects);

        // If this event is not repeatable,
        // remove it from the candidate event pool.
        if (!e.repeatable) {
            unlocked.events.Remove(e);
        }
    }


    void OnResearchCompleted(Discovery d) {
        ApplyEffectSet(d.effects);
    }

    private void ApplyEffectSet(EffectSet es) {
        playerCompany.ApplyBuffs(es.company);

        foreach (Worker worker in playerCompany.workers) {
            worker.ApplyBuffs(es.workers);
        }

        foreach (ProductEffect pe in es.products) {
            playerCompany.ApplyProductEffect(pe);
        }

        unlocked.Unlock(es.unlocks);
    }

    public bool HireConsultancy(Consultancy c) {
        // You pay the consultancy cost initially when hired, then repeated monthly.
        if (playerCompany.Pay(c.cost)) {
            researchManager.consultancy = c;
            playerCompany.consultancy = c;
            return true;
        }
        return false;
    }
}


