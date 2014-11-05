/*
 * UIManager
 * ================
 *
 * Handles persistent UI elements, such
 * as the menu, manages popups, and coordinates
 * other UI elements.
 *
 */

using UnityEngine;
using System.Collections;

public class UIManager : Singleton<UIManager> {
    private GameManager gm;

    public GameObject menu;
    private GameObject currentPopup;

    public GameObject gameEventNotificationPrefab;

    public GameObject alertPrefab;
    public GameObject confirmPrefab;
    public GameObject effectAlertPrefab;
    public GameObject researchCompletedAlertPrefab;
    public GameObject productCompletedAlertPrefab;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
        ResearchManager.Completed += OnResearchCompleted;
        Product.Completed += OnProductCompleted;

        //Confirm("thisthis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long message is a very long message");
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        ResearchManager.Completed -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
    }

    void OnEvent(GameEvent e) {
        UIGameEventNotification gameEventNotification = NGUITools.AddChild(gameObject, gameEventNotificationPrefab).GetComponent<UIGameEventNotification>();
        gameEventNotification.gameEvent = e;
    }

    public void ToggleMenu() {
        if (menu.activeInHierarchy) {
            menu.SetActive(false);
        } else {
            menu.SetActive(true);
        }
    }

    void OnResearchCompleted(Discovery d) {
        GameObject popup = NGUITools.AddChild(gameObject, researchCompletedAlertPrefab);
        popup.GetComponent<UIResearchCompletedAlert>().discovery = d;
    }
    void OnProductCompleted(Product p) {
        GameObject popup = NGUITools.AddChild(gameObject, productCompletedAlertPrefab);
        popup.GetComponent<UIProductCompletedAlert>().product = p;
    }

    public void OpenPopup(GameObject popupPrefab) {
        currentPopup = NGUITools.AddChild(gameObject, popupPrefab);
        //closeButton.SetActive(true);
        menu.SetActive(false);
    }

    public void ClosePopup() {
        NGUITools.DestroyImmediate(currentPopup);
        currentPopup = null;
        //closeButton.SetActive(false);
    }

    public UIAlert Alert(string text) {
        UIAlert alert = NGUITools.AddChild(gameObject, alertPrefab).GetComponent<UIAlert>();
        alert.bodyText = text;
        return alert;
    }

    public UIEffectAlert EffectAlert(string text, EffectSet es) {
        UIEffectAlert alert = NGUITools.AddChild(gameObject, effectAlertPrefab).GetComponent<UIEffectAlert>();
        alert.bodyText = text;
        alert.RenderEffects(es);
        alert.AdjustEffectsHeight();
        return alert;
    }

    public UIConfirm Confirm(string text) {
        UIConfirm confirm = NGUITools.AddChild(gameObject, confirmPrefab).GetComponent<UIConfirm>();
        confirm.bodyText = text;
        return confirm;
    }

    void UpdateHUD() {
        //if (GameManager.Instance.playerCompany.researching) {
            //progressBar.value = GameManager.Instance.playerCompany.consultancy.researchProgress;
        //} else {
            //inProgressOverlay.SetActive(false);
        //}
    }
}


