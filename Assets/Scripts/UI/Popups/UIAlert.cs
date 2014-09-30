/*
 * A text alert popup with an OK button.
 */

using UnityEngine;
using System.Collections;

public class UIAlert : UIPopup {
    public GameObject window;
    public UITexture overlay;
    public UILabel bodyLabel;
    public UIWidget body;
    public string bodyText {
        set {
            bodyLabel.text = value;
            Extend(bodyLabel.height);
        }
    }

    private float overlayAlpha = 0.3f;

    void OnEnable() {
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
        Show(window);
        GameManager.Instance.Pause();
    }

    public void Close() {
        GameManager.Instance.Resume();
        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        Hide(window);
    }
    public void Close(GameObject obj) {
        Close();
    }

    private void Extend(int amount) {
        amount = (amount/2) + 50;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }

    private IEnumerator FadeOverlay(float from, float to) {
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
