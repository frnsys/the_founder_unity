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
        StartCoroutine(Fade(overlay, 0f, overlayAlpha));
        base.Show(window);
    }

    public void Close_() {
        StartCoroutine(Fade(overlay, overlayAlpha, 0f));
        Hide(window);
    }
}
