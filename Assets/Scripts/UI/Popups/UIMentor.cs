using UnityEngine;
using System.Collections;

// A guidance message from your mentor.
public class UIMentor : UIPopup {
    public GameObject box;
    public GameObject shadow;
    public GameObject model;
    public UITexture overlay;

    private float overlayAlpha = 0.004f;

    void OnEnable() {
        Show(box);
        Show(shadow);

        overlay.gameObject.SetActive(true);
        StartCoroutine(FadeOverlay(0f, overlayAlpha));
        StartCoroutine(ScaleModel(new Vector3(40, 0, 40), new Vector3(40, 40, 40)));

        GameManager.Instance.Pause();
    }

    public void Hide() {
        StartCoroutine(FadeOverlay(overlayAlpha, 0f));
        StartCoroutine(ScaleModel(model.transform.localScale, new Vector3(40, 0, 40)));
        base.Hide(box);
        base.Hide(shadow);

        GameManager.Instance.Resume();
    }

    public string message {
        set {
            box.transform.Find("Message").GetComponent<UILabel>().text = value;
        }
    }

    public Color backgroundColor {
        set {
            box.GetComponent<UITexture>().material.color = value;
        }
    }

    private IEnumerator FadeOverlay(float from, float to) {
        float step = 0.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            overlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    private IEnumerator ScaleModel(Vector3 from, Vector3 to) {
        // Scale the model up and down, with a little bounciness.
        float step = 0.05f;
        Vector3 to_ = to * 1.1f;
        for (float f = 0f; f <= 1f + step; f += step) {
            model.transform.localScale = Vector3.Lerp(from, to_, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        step = 0.2f;
        for (float f = 0f; f <= 1f + step; f += step) {
            model.transform.localScale = Vector3.Lerp(to_, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
