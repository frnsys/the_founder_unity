using UnityEngine;
using System.Collections;

// Attach this script to objects in the
// world that should launch a window.
public class UIObject : MonoBehaviour {
    public GameObject window;
    public float duration = 1.0F;

    void OnClick() {
        UIManager.Instance.OpenPopup(window);
    }
}
