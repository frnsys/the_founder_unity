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

public class UIManager : MonoBehaviour {
    private GameManager gm;

    public GameObject menu;
    public GameObject closeButton;
    private GameObject currentPopup;

    void OnEnable() {
        gm = GameManager.Instance;
    }

    public void ToggleMenu() {
        if (menu.activeInHierarchy) {
            menu.SetActive(false);
        } else {
            menu.SetActive(true);
        }
    }

    public void ShowNewProductFlow() {
        OpenPopup("UI/Products/New Product Popup");
    }

    public void ShowHireWorker() {
        OpenPopup("UI/Workers/Hire Worker Popup");
    }

    public void OpenPopup(string popupPrefabPath) {
        GameObject popupPrefab = Resources.Load(popupPrefabPath) as GameObject;
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


