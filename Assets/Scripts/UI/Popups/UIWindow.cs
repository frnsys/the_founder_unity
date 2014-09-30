/*
 * Superclass for popover game windows.
 * Supports two tabs.
 */

using UnityEngine;
using System.Collections;

public class UIWindow : UIPopup {
    private GameObject currentScreen_;
    public GameObject currentScreen {
        set {
            currentScreen_.SetActive(false);

            value.SetActive(true);
            currentScreen_ = value;
        }
    }

    public void SelectTab(GameObject screen) {
        currentScreen = screen;
    }

    void OnEnable() {
        Show(gameObject);
    }

    public void Close() {
        Hide(gameObject);
    }
}
