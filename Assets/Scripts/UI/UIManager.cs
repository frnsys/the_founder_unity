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
    public GameObject closeButton;
    private GameObject currentPopup;

    public GameObject gameEventNotificationPrefab;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        gm = GameManager.Instance;
        GameEvent.EventTriggered += OnEvent;
    }

    void OnDisable() {
        GameEvent.EventTriggered -= OnEvent;
    }

    void OnEvent(GameEvent e) {
        UIGameEventNotification gameEventNotification = NGUITools.AddChild(gameObject, gameEventNotificationPrefab).GetComponent<UIGameEventNotification>();
        gameEventNotification.gameEvent = e;

        // Pause
        Time.timeScale = 0;
    }

    public void ToggleMenu() {
        if (menu.activeInHierarchy) {
            menu.SetActive(false);
        } else {
            menu.SetActive(true);
        }
    }

    public void OpenPopup(GameObject popupPrefab) {
        currentPopup = NGUITools.AddChild(gameObject, popupPrefab);
        closeButton.SetActive(true);
        menu.SetActive(false);
    }

    public void ClosePopup() {
        NGUITools.DestroyImmediate(currentPopup);
        currentPopup = null;
        closeButton.SetActive(false);
    }
}


