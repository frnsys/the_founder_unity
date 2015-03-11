using UnityEngine;
using System.Collections;

public class UIMenuItem : MonoBehaviour {
    public GameObject window;
    public bool wiggle;
    private IEnumerator wiggler;

    void OnClick() {
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

    void OnEnable() {
        if (wiggle && wiggler == null) {
            wiggler = Pulse();
            StartCoroutine(wiggler);
        }
    }

    private IEnumerator Pulse() {
        Vector3 fromScale = new Vector3(1f, 1f, 1f);
        Vector3 toScale = new Vector3(1.05f, 1.05f, 1.05f);
        float step = 0.03f;

        while (true) {
            for (float f = 0f; f <= 1f + step; f += step) {
                transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }

            for (float f = 0f; f <= 1f + step; f += step) {
                transform.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
        }
    }
}
