using UnityEngine;
using System.Collections;

public class UIMenuButton : MonoBehaviour {
    private IEnumerator wiggler;

    void OnClick() {
        if (wiggler != null) {
            StopCoroutine(wiggler);
            wiggler = null;
        }
    }

    public void Wiggle() {
        if (wiggler == null) {
            wiggler = Pulse();
            StartCoroutine(wiggler);
        }
    }

    private IEnumerator Pulse() {
        Vector3 fromScale = new Vector3(1f, 1f, 1f);
        Vector3 toScale = new Vector3(1.1f, 1.1f, 1.1f);
        float step = 0.024f;

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
