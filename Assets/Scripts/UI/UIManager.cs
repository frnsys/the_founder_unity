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
    public GameObject interlude;
    public UIInterlude Interlude {
        get { return interlude.GetComponent<UIInterlude>(); }
    }

    public GameObject eventPersonalPrefab;
    public GameObject eventEmailPrefab;
    public GameObject eventNewsPrefab;

    public GameObject alertPrefab;
    public GameObject confirmPrefab;
    public GameObject effectAlertPrefab;
    public GameObject annualReportPrefab;
    public GameObject productInfoAlertPrefab;
    public GameObject productDiscoveredAlertPrefab;
    public GameObject specialProjectCompletedAlertPrefab;
    public GameObject hiringPrefab;

    public UIMenu menu;
    public void OpenMenu() {
        menu.gameObject.SetActive(true);
    }
    public void CloseMenu() {
        menu.gameObject.SetActive(false);
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        SpecialProject.Completed += OnSpecialProjectCompleted;
        Recruitment.Completed += OnRecruitmentCompleted;
        GameManager.YearEnded += OnYearEnded;
        GameManager.PerformanceReport += OnPerformanceReport;
        MainGame.Done += OnGridGameDone;
        Company.DiscoveredProduct += OnDiscoveredProduct;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        SpecialProject.Completed -= OnSpecialProjectCompleted;
        Recruitment.Completed -= OnRecruitmentCompleted;
        GameManager.YearEnded -= OnYearEnded;
        GameManager.PerformanceReport -= OnPerformanceReport;
        MainGame.Done -= OnGridGameDone;
        Company.DiscoveredProduct -= OnDiscoveredProduct;
    }

    void OnDiscoveredProduct(Company c, Product p) {
        if (c == gm.playerCompany) {
            GameObject popup = NGUITools.AddChild(alertsPanel, productDiscoveredAlertPrefab);
            popup.GetComponent<UIProductDiscoveredAlert>().product = p;
        }
    }

    public void ShowProductInfo(ProductRecipe r) {
        GameObject popup = NGUITools.AddChild(alertsPanel, productInfoAlertPrefab);
        popup.GetComponent<UIProductDiscoveredAlert>().recipe = r;
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
        List<AWorker> workers = gm.workerManager.WorkersForRecruitment(r);
        Alert(string.Format("Our recruiting has finished. We had {0} applicants. Here is the info we have on them.", workers.Count));
        GameObject window = NGUITools.AddChild(windowsPanel, hiringPrefab);
        window.GetComponent<UIWidget>().SetAnchor(windowsPanel.gameObject, 0, 0, 0, 0);
        window.GetComponent<UIHireWorkers>().LoadWorkers(workers);
    }

    void OnPerformanceReport(int year, Company.StatusReport report, TheBoard board) {
        PerformanceReport(report, board);
    }

    void OnYearEnded(int year) {
        // Only show every 10th birthday.
        if (gm.age % 10 == 0) {
            // TO DO this should be a notice event.
            UIManager.Instance.Alert(
                string.Format("Happy {0}th birthday! I called the doctor today - she estimates you'll live another {1}-{2} years. Can you believe that we founded this company {3} years ago?", gm.age, 40-gm.age, 60-gm.age, gm.age-25)
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

    public bool isDisplaying {
        get {
            return currentPopup != null || alertsPanel.transform.childCount > 0;
        }
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
    public UIPerformanceReport PerformanceReport(Company.StatusReport statusReport, TheBoard board) {
        UIPerformanceReport report = NGUITools.AddChild(alertsPanel, annualReportPrefab).GetComponent<UIPerformanceReport>();
        report.BuildReport(statusReport, board);
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
    //public GameObject selectProductTypesWindow; // TODO adapt this window to office production selection
    public void SetupGridGame() {
        officeCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(false);
        interlude.SetActive(false);

        List<ProductType> productTypes = gm.playerCompany.productTypes;
        AAICompany opponent = gm.otherCompanies[UnityEngine.Random.Range(0, gm.otherCompanies.Count - 1)];
        StartGridGame(productTypes, opponent);
    }

    public void StartGridGame(List<ProductType> productTypes, AAICompany opponent) {
        gridGame.gameObject.SetActive(true);
        gridGame.Setup(productTypes, opponent);
    }

    void OnGridGameDone() {
        officeCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(true);
        gridGame.gameObject.SetActive(false);
        interlude.SetActive(true);
    }
}


