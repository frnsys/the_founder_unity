using UnityEngine;
using System.Collections;

public class UIObject : MonoBehaviour {
    public GameObject window;
    public float duration = 1.0F;

    void OnClick() {
        UIManager.Instance.OpenPopup(window);
    }
}
