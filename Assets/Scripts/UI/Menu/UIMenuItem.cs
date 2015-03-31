using UnityEngine;
using System.Collections;

public class UIMenuItem : MonoBehaviour {
    public GameObject window;
    public bool wiggle;
    private IEnumerator wiggler;
    public GameObject display;
    public bool locked;

    [HideInInspector]
    public Material mat;

    void OnClick() {
        if (!locked) {
            if (wiggler != null) {
                StopCoroutine(wiggler);
                wiggler = null;
                wiggle = false;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }

            UIManager.Instance.CloseMenu();

            if (window != null)
                UIManager.Instance.OpenPopup(window);
        }
    }

    void OnEnable() {
        if (wiggle && wiggler == null) {
            wiggler = UIAnimator.PulseUI(transform, 1f, 1.25f, 4f);
            StartCoroutine(wiggler);
        }
    }

    void Update() {
        if (display != null)
            UIAnimator.Rotate(display);
    }
}
