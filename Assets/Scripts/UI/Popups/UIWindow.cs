/*
 * Superclass for popover game windows.
 * Supports two tabs.
 */

using UnityEngine;
using System.Collections;

public class UIWindow : UIPopup {
    // An event that fires when a window is opened.
    static public event System.Action<string> WindowOpened;

    void OnEnable() {
        Show(gameObject);

        if (GameManager.hasInstance)
            GameManager.Instance.Pause();

        if (WindowOpened != null)
            WindowOpened(gameObject.name.Replace("(Clone)", ""));
    }

    public void Close() {
        // no scaling animation
        // Hide(gameObject);
        NGUITools.DestroyImmediate(gameObject);
    }

    void OnDisable() {
        if (GameManager.hasInstance)
            GameManager.Instance.Resume();
    }

    public void Show(GameObject target) {
        // no scaling animation
    }
}
