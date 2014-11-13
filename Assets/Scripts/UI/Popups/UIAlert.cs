using UnityEngine;
using System.Collections;

// A text alert popup with an OK button.
public class UIAlert : UIPopup {
    public GameObject window;
    public UITexture overlay;
    public UIWidget body;
    public UILabel bodyLabel;
    public string bodyText {
        set {
            bodyLabel.text = value;
            Extend(bodyLabel.height);
        }
    }

    private float overlayAlpha = 0.3f;

    void OnEnable() {
        Show();
    }

    // As of v3.7.4, NGUI cannot handle inherited overloaded methods
    // when using them as button events. So we avoid using overloaded methods.
    public void Close_() {
        if (GameManager.hasInstance)
            GameManager.Instance.Resume();

        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        Hide(window);
    }
    public void Close(GameObject obj) {
        Close_();
    }

    public void Show() {
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
        base.Show(window);

        if (GameManager.hasInstance)
            GameManager.Instance.Pause();
    }

    protected virtual void Extend(int amount) {
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
