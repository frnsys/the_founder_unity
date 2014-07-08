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
        GameObject newProductFlow = Resources.Load("UI/Products/New Product Selection") as GameObject;
        popup = NGUITools.AddChild(gameObject, newProductFlow);
        closeButton.SetActive(true);
    }

    public void ShowHireWorker() {
        // to do
    }

    public void ClosePopup() {
        NGUITools.DestroyImmediate(popup);
        popup = null;
        closeButton.SetActive(false);
    }
}


