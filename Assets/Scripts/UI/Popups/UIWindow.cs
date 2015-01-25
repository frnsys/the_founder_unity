/*
 * Superclass for popover game windows.
 * Supports two tabs.
 */

using UnityEngine;
using System.Collections;

public class UIWindow : UIPopup {
    // An event that fires when a window is opened.
    static public event System.Action<string> WindowOpened;
    static public event System.Action<string> TabOpened;

    protected GameObject currentScreen_;
    public GameObject currentScreen {
        set {
            if (currentScreen_ != value) {
                currentScreen_.SetActive(false);
                value.SetActive(true);
                currentScreen_ = value;
            }
        }
    }

    public void SelectTab(GameObject screen) {
        currentScreen = screen;
        if (TabOpened != null)
            TabOpened(screen.name.Replace("(Clone)", ""));
    }

    void OnEnable() {
        Show(gameObject);
        if (WindowOpened != null)
            WindowOpened(gameObject.name.Replace("(Clone)", ""));
    }

    public void Close() {
        Hide(gameObject);
    }
}
