using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISelectPopup : UIPopup {
    public GameObject window;
    public UITexture overlay;
    public UICenteredGrid grid;
    public UIScrollView scrollView;
    public GameObject itemPrefab;

    private float overlayAlpha = 0.3f;

    void OnEnable() {
        Show();
    }

    public void Show() {
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
        base.Show(window);
    }

    public void Close_() {
        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        Hide(window);
    }

    private IEnumerator FadeOverlay(float from, float to) {
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
