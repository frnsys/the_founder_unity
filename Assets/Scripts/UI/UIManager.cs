using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    private GameManager gm;

    public GameObject menu;
    public GameObject closeButton;

    // Currently open popup.
    private GameObject popup;

    void OnEnable() {
        gm = GameManager.Instance;
    }

    void Update() {
    }

    public void TestLog() {
        Debug.Log("CLICKED");
    }

    public void ToggleMenu() {
        if (menu.activeInHierarchy) {
            menu.SetActive(false);
        } else {
            menu.SetActive(true);
        }
    }

    public void ShowNewProductFlow() {
        OpenPopup("UI/Products/New Product Selection");
    }

    public void ShowHireWorker() {
        OpenPopup("UI/Workers/Hire Workers");
    }

    public void OpenPopup(string popupPrefabPath) {
        GameObject popupPrefab = Resources.Load(popupPrefabPath) as GameObject;
        popup = NGUITools.AddChild(gameObject, popupPrefab);
        closeButton.SetActive(true);
        menu.SetActive(false);
    }

    public void ClosePopup() {
        NGUITools.DestroyImmediate(popup);
        popup = null;
        closeButton.SetActive(false);
    }
}


