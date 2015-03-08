using UnityEngine;
using System.Collections;

public class UIMenuItem : MonoBehaviour {
    public GameObject window;

    void OnClick() {
        UIManager.Instance.CloseMenu();
        UIManager.Instance.OpenPopup(window);
    }
}
