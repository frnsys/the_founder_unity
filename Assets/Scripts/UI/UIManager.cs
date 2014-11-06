/*
 * Handles persistent UI elements, such
 * as the menu, manages popups, and coordinates
 * other UI elements.
 *
 */

using UnityEngine;
using System;
using System.Collections;

public class UIManager : Singleton<UIManager> {
    private GameManager gm;

    public GameObject menu;

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
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
        ResearchManager.Completed -= OnResearchCompleted;
        Product.Completed -= OnProductCompleted;
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
        UIGameEventNotification gameEventNotification = NGUITools.AddChild(gameObject, gameEventNotificationPrefab).GetComponent<UIGameEventNotification>();
        gameEventNotification.gameEvent = e;
    }

    // Show a "research completed" alert.
    void OnResearchCompleted(Discovery d) {
        GameObject popup = NGUITools.AddChild(gameObject, researchCompletedAlertPrefab);
        popup.GetComponent<UIResearchCompletedAlert>().discovery = d;
    }

    // Show a "product completed" alert.
    void OnProductCompleted(Product p) {
        GameObject popup = NGUITools.AddChild(gameObject, productCompletedAlertPrefab);
        popup.GetComponent<UIProductCompletedAlert>().product = p;
    }

    public GameObject currentPopup;
    public void OpenPopup(GameObject popupPrefab) {
        currentPopup = NGUITools.AddChild(gameObject, popupPrefab);
        menu.SetActive(false);
    }
    public void ClosePopup() {
        NGUITools.DestroyImmediate(currentPopup);
        currentPopup = null;
    }

    // Create a simple alert.
    public UIAlert Alert(string text) {
        UIAlert alert = NGUITools.AddChild(gameObject, alertPrefab).GetComponent<UIAlert>();
        alert.bodyText = text;
        return alert;
    }

    // Create an alert which lists some effects.
    public UIEffectAlert EffectAlert(string text, EffectSet es) {
        UIEffectAlert alert = NGUITools.AddChild(gameObject, effectAlertPrefab).GetComponent<UIEffectAlert>();
        alert.bodyText = text;
        alert.RenderEffects(es);
        alert.AdjustEffectsHeight();
        return alert;
    }

    // Create a confirmation popup.
    public UIConfirm Confirm(string text, Action yes, Action no) {
        UIConfirm confirm = NGUITools.AddChild(gameObject, confirmPrefab).GetComponent<UIConfirm>();
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
}


