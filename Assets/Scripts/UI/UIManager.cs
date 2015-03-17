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
    public ProductMinigame pm;
    public void LaunchProductMinigame(Product p) {
        pm.gameObject.SetActive(true);
        pm.Setup(p);
    }

    private GameManager gm;

    public Camera uiCamera;
    public TheMarket theMarket;
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
    public GameObject annualReportPrefab;
    public GameObject productCompletedAlertPrefab;
    public GameObject specialProjectCompletedAlertPrefab;
    public GameObject competitorProductCompletedAlertPrefab;
    public GameObject hiringPrefab;

    public UIMenu menu;
    public GameObject menuButton;
    public void OpenMenu() {
        menu.gameObject.SetActive(true);
    }
    public void CloseMenu() {
        menu.gameObject.SetActive(false);
    }
    public UIStatusBar statusBar;

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
                yield return StartCoroutine(GameTimer.Wait(2f));
                NGUITools.Destroy(ping.gameObject);
            }
            yield return StartCoroutine(GameTimer.Wait(0.5f));
        }
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        Product.Completed += OnProductCompleted;
        SpecialProject.Completed += OnSpecialProjectCompleted;
        Recruitment.Completed += OnRecruitmentCompleted;
        GameManager.YearEnded += OnYearEnded;
        GameManager.PerformanceReport += OnPerformanceReport;
        Company.Paid += OnPaid;
        Company.BeganProduct += OnBeganProduct;

        pendingPings = new Queue<Ping>();
        StartCoroutine(ShowPings());
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        Product.Completed -= OnProductCompleted;
        SpecialProject.Completed -= OnSpecialProjectCompleted;
        Recruitment.Completed -= OnRecruitmentCompleted;
        GameManager.YearEnded -= OnYearEnded;
        GameManager.PerformanceReport -= OnPerformanceReport;
        Company.Paid -= OnPaid;
        Company.BeganProduct -= OnBeganProduct;
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

    void OnBeganProduct(Product p, Company c) {
        if (c == gm.playerCompany) {
            productHud.SetActive(true);
            hud.SetActive(false);
            LaunchProductMinigame(p);
        }
    }

    // Show a "product completed" alert.
    void OnProductCompleted(Product p, Company c) {
        // For the player's products, show the product completed alert.
        if (c == gm.playerCompany) {
            GameObject popup = NGUITools.AddChild(alertsPanel, productCompletedAlertPrefab);
            popup.GetComponent<UIProductCompletedAlert>().product = p;

            // Clear/hide the product HUD.
            hud.SetActive(true);
            productHud.SetActive(false);

            // Notify the player that they were missing a tech.
            if (p.techPenalty)
                GameManager.Instance.eventManager.DelayTrigger(GameEvent.LoadNoticeEvent("Missing Technology"), 25f);

            // Hack to show The Market after the player has hit OK on the product completed alert.
            StartCoroutine(Delay(delegate {
                theMarket.Setup(p);
            }, 0.6f));

        // If it is a competitor's product, show it as an "ad".
        } else {
            GameObject popup = NGUITools.AddChild(alertsPanel, competitorProductCompletedAlertPrefab);
            popup.GetComponent<UIProductAdAlert>().SetProductAndCompany(p, c);
        }
    }

    // Show a "special project completed" alert.
    void OnSpecialProjectCompleted(SpecialProject p) {
        GameObject popup = NGUITools.AddChild(alertsPanel, specialProjectCompletedAlertPrefab);
        popup.GetComponent<UISpecialProjectCompletedAlert>().specialProject = p;
    }


    void OnRecruitmentCompleted(Recruitment r) {
        List<Worker> workers = GameManager.Instance.workerManager.WorkersForRecruitment(r);
        Alert(string.Format("Our recruiting has finished. We had {0} applicants. Here is the info we have on them.", workers.Count));
        GameObject window = NGUITools.AddChild(windowsPanel, hiringPrefab);
        window.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
        window.GetComponent<UIHireWorkers>().LoadWorkers(workers);
    }

    void OnPerformanceReport(int year, PerformanceDict results, PerformanceDict deltas, TheBoard board) {
        PerformanceReport(results, deltas, board);
    }

    void OnYearEnded(int year) {
        // Anniversary/birthday alert!
        int age = 25 + year;
        // Only show every 10th birthday.
        if (age % 10 == 0) {
            // TO DO this should be a notice event.
            UIManager.Instance.Alert(
                string.Format("Happy {0}th birthday! I called the doctor today - she estimates you'll live another {1}-{2} years. Can you believe that we founded this company {3} years ago?", age, 40-age, 60-age, age-25)
            );
        }
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
    public void CloseAndOpenPopup(GameObject popupPrefab) {
        ClosePopup();
        OpenPopup(popupPrefab);
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
        UIPerformanceReport report = NGUITools.AddChild(alertsPanel, annualReportPrefab).GetComponent<UIPerformanceReport>();
        report.BuildReport(results, deltas, board);
        return report;
    }

    public GameObject hypeMinigame;
    public void LaunchHypeMinigame(Promo promo) {
        GameObject hmg = NGUITools.AddChild(gameObject, hypeMinigame);
        hmg.GetComponent<HypeMinigame>().Setup(promo);

        foreach (UIFollowTarget uift in hmg.GetComponentsInChildren<UIFollowTarget>()) {
            uift.gameCamera = uiCamera;
            uift.uiCamera = uiCamera;
        }
    }

    public GameObject productHud;
    public GameObject hud;
    public void AddPointsToDevelopingProduct(string feature, float value) {
        //productHud.Add(feature, (int)value);
    }

    private IEnumerator Delay(UIEventListener.VoidDelegate callback, float delay = 12f) {
        yield return StartCoroutine(GameTimer.Wait(delay));
        callback(null);
    }
}


