using UnityEngine;
using System.Collections;

public class UIMenuItem : MonoBehaviour {
    public GameObject window;

    void OnClick() {
        UIManager.Instance.CloseMenu();

        if (window != null)
            UIManager.Instance.OpenPopup(window);
    }
}
