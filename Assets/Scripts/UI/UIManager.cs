/*
 * Handles persistent UI elements, such
 * as the menu, manages popups, and coordinates
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

    public GameObject menu;
    public GameObject windowsPanel;
    public GameObject alertsPanel;

    public GameObject gameEventNotificationPrefab;

    public GameObject alertPrefab;
    public GameObject confirmPrefab;
    public GameObject emailPrefab;
    public GameObject effectAlertPrefab;
    public GameObject annualReportPrefab;
    public GameObject researchCompletedAlertPrefab;
    public GameObject productCompletedAlertPrefab;
    public GameObject competitorProductCompletedAlertPrefab;
    public GameObject selectWorkerPopupPrefab;
    public GameObject selectPromoPopupPrefab;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        ResearchManager.Completed += OnResearchCompleted;
        Product.Completed += OnProductCompleted;
        Promo.Completed += OnPromoCompleted;
        GameManager.YearEnded += OnYearEnded;
        GameManager.GameLost += OnGameLost;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        ResearchManager.Completed -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
        Promo.Completed -= OnPromoCompleted;
        GameManager.YearEnded -= OnYearEnded;
        GameManager.GameLost -= OnGameLost;
    }

    public void ToggleMenu() {
        if (menu.activeInHierarchy) {
            menu.SetActive(false);
        } else {
            menu.SetActive(true);
        }
    }

    // Show an event notification.
    void OnEvent(GameEvent e) {
        UIGameEventNotification gameEventNotification = NGUITools.AddChild(alertsPanel, gameEventNotificationPrefab).GetComponent<UIGameEventNotification>();
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

    void OnPromoCompleted(Promo p) {
        Alert(p.name + " completed.");
    }

    void OnYearEnded(int year, PerformanceDict results, PerformanceDict deltas, TheBoard board) {
        AnnualReport(results, deltas, board);

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
        UIManager.Instance.Alert("Happy " + year + ending + " birthday!");
    }

    void OnGameLost(Company company) {
        // TO DO this should be a proper "lose game"
        Alert("YOU LOSE");
    }

    [HideInInspector]
    public GameObject currentPopup;
    public void OpenPopup(GameObject popupPrefab) {
        currentPopup = NGUITools.AddChild(windowsPanel, popupPrefab);

        // Set to be full screen.
        currentPopup.transform.localScale = Vector3.zero;
        currentPopup.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
        menu.SetActive(false);
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
    public UIAnnualReport AnnualReport(PerformanceDict results, PerformanceDict deltas, TheBoard board) {
        UIAnnualReport report = NGUITools.AddChild(alertsPanel, annualReportPrefab).GetComponent<UIAnnualReport>();
        report.BuildReport(results, deltas, board);
        return report;
    }

    // Create an email.
    public UIEmail Email(string from, string to, string subject, string message) {
        UIEmail email = NGUITools.AddChild(alertsPanel, emailPrefab).GetComponent<UIEmail>();
        email.SetHeaders(from, to, subject);
        email.bodyText = message;
        return email;
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

    // Show a promo selection popup.
    public void PromoSelectionPopup(Action<Promo> confirm) {
        UISelectPromoPopup p = NGUITools.AddChild(alertsPanel, selectPromoPopupPrefab).GetComponent<UISelectPromoPopup>();
        p.SetData(GameManager.Instance.unlocked.promos, confirm);
    }
}


