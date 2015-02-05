using UnityEngine;
using System.Collections;

// A guidance message from your mentor.
public class UIMentor : UIPopup {
    // Note: this doesn't pause the game. should it?

    public GameObject box;
    public GameObject shadow;
    public GameObject model;
    public UITexture overlay;

    private float overlayAlpha = 0.3f;

    void OnEnable() {
        Show(box);
        Show(shadow);
    }

    public void Interrupt() {
        overlay.gameObject.SetActive(true);
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
    }

    public void Hide() {
        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        base.Hide(box);
        base.Hide(shadow);

        // TO DO animate out the mentor model
    }

    public string message {
        set {
            box.transform.Find("Message").GetComponent<UILabel>().text = value;
        }
    }

    private IEnumerator FadeOverlay(float from, float to) {
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
