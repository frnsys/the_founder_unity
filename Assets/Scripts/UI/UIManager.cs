/*
 * Handles persistent UI elements, such
 * as popups and coordinates
 * other UI elements.
 *
 */

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager> {
    private GameManager gm;

    public GameObject windowsPanel;
    public GameObject alertsPanel;
    public GameObject pingsPanel;

    public GameObject eventPersonalPrefab;
    public GameObject eventEmailPrefab;
    public GameObject eventNewsPrefab;
    public GameObject pingPrefab;
    private Queue<Ping> pendingPings;

    public GameObject alertPrefab;
    public GameObject confirmPrefab;
    public GameObject effectAlertPrefab;
    public GameObject quarterlyReportPrefab;
    public GameObject researchCompletedAlertPrefab;
    public GameObject productCompletedAlertPrefab;
    public GameObject competitorProductCompletedAlertPrefab;
    public GameObject selectWorkerPopupPrefab;
    public GameObject hiringPrefab;

    private struct Ping {
        public string note;
        public Color color;
        public Ping(string n, Color c) {
            note = n;
            color = c;
        }
    }

    public void SendPing(string note, Color color) {
        pendingPings.Enqueue(new Ping(note, color));
    }

    IEnumerator ShowPings() {
        while(true) {
            while (pendingPings.Count > 0) {
                Ping p = pendingPings.Dequeue();
                UIPing ping = NGUITools.AddChild(pingsPanel, pingPrefab).GetComponent<UIPing>();
                ping.Set(p.note, p.color);
                yield return new WaitForSeconds(2f);
                NGUITools.Destroy(ping.gameObject);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        ResearchManager.Completed += OnResearchCompleted;
        Product.Completed += OnProductCompleted;
        Recruitment.Completed += OnRecruitmentCompleted;
        GameManager.YearEnded += OnYearEnded;
        GameManager.PerformanceReport += OnPerformanceReport;
        GameManager.GameLost += OnGameLost;
        Company.Paid += OnPaid;

        pendingPings = new Queue<Ping>();
        StartCoroutine(ShowPings());
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        ResearchManager.Completed -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
        Recruitment.Completed -= OnRecruitmentCompleted;
        GameManager.YearEnded -= OnYearEnded;
        GameManager.PerformanceReport -= OnPerformanceReport;
        GameManager.GameLost -= OnGameLost;
        Company.Paid -= OnPaid;
    }

    void OnPaid(float amount, string name) {
        SendPing(string.Format("Paid {0:C0} {1}", amount, name), Color.red);
    }

    // Show an event notification.
    void OnEvent(GameEvent e) {
        GameObject prefab;
        switch (e.type) {
            case (GameEvent.Type.Email):
                prefab = eventEmailPrefab;
                break;
            case (GameEvent.Type.Personal):
                prefab = eventPersonalPrefab;
                break;
            case (GameEvent.Type.News):
                prefab = eventNewsPrefab;
                break;
            default:
                prefab = eventNewsPrefab;
                break;
        }
        UIGameEventNotification gameEventNotification = NGUITools.AddChild(alertsPanel, prefab).GetComponent<UIGameEventNotification>();
        gameEventNotification.gameEvent = e;
    }

    // Show a "research completed" alert.
    void OnResearchCompleted(Technology t) {
        GameObject popup = NGUITools.AddChild(alertsPanel, researchCompletedAlertPrefab);
        popup.GetComponent<UIResearchCompletedAlert>().technology = t;
    }

    // Show a "product completed" alert.
    void OnProductCompleted(Product p, Company c) {
        // For the player's products, show the product completed alert.
        if (c == gm.playerCompany) {
            GameObject popup = NGUITools.AddChild(alertsPanel, productCompletedAlertPrefab);
            popup.GetComponent<UIProductCompletedAlert>().product = p;

        // If it is a competitor's product, show it as an "ad".
        } else {
            GameObject popup = NGUITools.AddChild(alertsPanel, competitorProductCompletedAlertPrefab);
            popup.GetComponent<UIProductAdAlert>().SetProductAndCompany(p, c);
        }
    }

    void OnRecruitmentCompleted(Recruitment r) {
        IEnumerable<Worker> workers = GameManager.Instance.workerManager.WorkersForRecruitment(r);
        Alert("Our recruiting has finished. We had " + workers.Count().ToString() + " applicants. Here is the info we have on them.");
        GameObject window = NGUITools.AddChild(windowsPanel, hiringPrefab);
        window.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
        window.GetComponent<UIHireWorkers>().LoadWorkers(workers);
    }

    void OnPerformanceReport(int quarter, PerformanceDict results, PerformanceDict deltas, TheBoard board) {
        PerformanceReport(results, deltas, board);
    }

    void OnYearEnded(int year) {
        // Anniversary/birthday alert!
        int age = 25 + year;
        int lastDigit = age % 10;
        string ending = "th";
        if (lastDigit == 1)
            ending = "st";
        else if (lastDigit == 2)
            ending = "nd";
        else if (lastDigit == 3)
            ending = "rd";

        UIManager.Instance.Alert(
            string.Format("Happy {0}{1} birthday! I called the doctor today - she estimates you'll live another {2}-{3} years.", age, ending, 40-age, 60-age)
        );
    }

    void OnGameLost(Company company) {
        Alert("Appalled by your inability to maintain the growth they are legally entitled to, the board has forced your resignation. You lose.");
        Alert("But you don't really lose. Because you have secured your place in a class shielded from any real consequence or harm. You'll be fine. You could always found another company.");
    }

    [HideInInspector]
    public GameObject currentPopup;
    public void OpenPopup(GameObject popupPrefab) {
        if (currentPopup != null)
            return;

        currentPopup = NGUITools.AddChild(windowsPanel, popupPrefab);

        // Set to be full screen.
        // (no scaling)
        // currentPopup.transform.localScale = Vector3.zero;
        currentPopup.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
    }
    public void ClosePopup() {
        NGUITools.DestroyImmediate(currentPopup);
        currentPopup = null;
    }

    // Create a simple alert.
    public UIAlert Alert(string text) {
        UIAlert alert = NGUITools.AddChild(alertsPanel, alertPrefab).GetComponent<UIAlert>();
        alert.bodyText = text;
        return alert;
    }

    // Create an alert which lists some effects.
    public UIEffectAlert EffectAlert(string text, EffectSet es) {
        UIEffectAlert alert = NGUITools.AddChild(alertsPanel, effectAlertPrefab).GetComponent<UIEffectAlert>();
        alert.bodyText = text;
        alert.RenderEffects(es);
        alert.AdjustEffectsHeight();
        return alert;
    }

    // Create a confirmation popup.
    public UIConfirm Confirm(string text, Action yes, Action no) {
        UIConfirm confirm = NGUITools.AddChild(alertsPanel, confirmPrefab).GetComponent<UIConfirm>();
        confirm.bodyText = text;

        UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
            yes();
            confirm.Close_();
        };

        // The `no` method can be null if you just want nothing to happen.
        UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
            if (no != null)
                no();
            confirm.Close_();
        };

        UIEventListener.Get(confirm.yesButton).onClick += yesAction;
        UIEventListener.Get(confirm.noButton).onClick += noAction;

        return confirm;
    }

    // Create an annual report.
    public UIPerformanceReport PerformanceReport(PerformanceDict results, PerformanceDict deltas, TheBoard board) {
        UIPerformanceReport report = NGUITools.AddChild(alertsPanel, quarterlyReportPrefab).GetComponent<UIPerformanceReport>();
        report.BuildReport(results, deltas, board);
        return report;
    }

    // Show a worker selection popup.
    public void WorkerSelectionPopup(string title, Action<Worker> confirm, Func<Worker, bool> filter, Worker selected) {
        IEnumerable<Worker> workers;
        if (filter != null) {
            workers = GameManager.Instance.playerCompany.workers.Where(w => filter(w));
        } else {
            workers = GameManager.Instance.playerCompany.workers;
        }

        UISelectWorkerPopup p = NGUITools.AddChild(alertsPanel, selectWorkerPopupPrefab).GetComponent<UISelectWorkerPopup>();
        p.SetData(title, workers, confirm, selected);
    }

}


