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

    public Camera uiCamera;
    public TheMarket theMarket;
    public GameObject windowsPanel;
    public GameObject alertsPanel;

    public GameObject eventPersonalPrefab;
    public GameObject eventEmailPrefab;
    public GameObject eventNewsPrefab;

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

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        SpecialProject.Completed += OnSpecialProjectCompleted;
        Recruitment.Completed += OnRecruitmentCompleted;
        GameManager.YearEnded += OnYearEnded;
        GameManager.PerformanceReport += OnPerformanceReport;
        MainGame.Done += OnGridGameDone;
        //Company.Paid += OnPaid;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        SpecialProject.Completed -= OnSpecialProjectCompleted;
        Recruitment.Completed -= OnRecruitmentCompleted;
        GameManager.YearEnded -= OnYearEnded;
        GameManager.PerformanceReport -= OnPerformanceReport;
        MainGame.Done -= OnGridGameDone;
        //Company.Paid -= OnPaid;
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

    // Show a "special project completed" alert.
    void OnSpecialProjectCompleted(SpecialProject p) {
        GameObject popup = NGUITools.AddChild(alertsPanel, specialProjectCompletedAlertPrefab);
        popup.GetComponent<UISpecialProjectCompletedAlert>().specialProject = p;
    }

    void OnRecruitmentCompleted(Recruitment r) {
        List<AWorker> workers = GameManager.Instance.workerManager.WorkersForRecruitment(r);
        Alert(string.Format("Our recruiting has finished. We had {0} applicants. Here is the info we have on them.", workers.Count));
        GameObject window = NGUITools.AddChild(windowsPanel, hiringPrefab);
        window.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
        window.GetComponent<UIHireWorkers>().LoadWorkers(workers);
    }

    void OnPerformanceReport(int year, Dictionary<string, float> results, TheBoard board) {
        PerformanceReport(results, board);
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
    public UIPerformanceReport PerformanceReport(Dictionary<string, float> results, TheBoard board) {
        UIPerformanceReport report = NGUITools.AddChild(alertsPanel, annualReportPrefab).GetComponent<UIPerformanceReport>();
        report.BuildReport(results, board);
        return report;
    }

    public GameObject promoWindow;
    public void ShowPromos() {
        OpenPopup(promoWindow);
    }

    private IEnumerator Delay(UIEventListener.VoidDelegate callback, float delay = 12f) {
        yield return StartCoroutine(GameTimer.Wait(delay));
        callback(null);
    }

    public MainGame gridGame;
    public Camera officeCamera;
    public Camera mainCamera;
    public void StartGridGame(GameObject obj) {
        officeCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(false);
        gridGame.gameObject.SetActive(true);
    }

    void OnGridGameDone() {
        officeCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        gridGame.gameObject.SetActive(false);
    }
}


