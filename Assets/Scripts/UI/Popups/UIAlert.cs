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

        StartCoroutine(Fade(overlay, overlayAlpha, 0f));
        Hide(window);
    }
    public void Close(GameObject obj) {
        Close_();
    }

    public void Show() {
        StartCoroutine(Fade(overlay, 0f, overlayAlpha));

        base.Show(window);

        if (GameManager.hasInstance)
            GameManager.Instance.Pause();
    }

    protected virtual void Extend(int amount) {
        amount = (amount/2) + 75;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }
}

