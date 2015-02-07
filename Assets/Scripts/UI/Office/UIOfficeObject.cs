using UnityEngine;
using System.Collections;

// Attach this script to objects in the
// world that should launch a window.
public class UIOfficeObject : MonoBehaviour {
    public GameObject window;

    void OnClick() {
        UIManager.Instance.OpenPopup(window);
    }
}
