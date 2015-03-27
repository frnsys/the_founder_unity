using UnityEngine;
using System.Collections;

// A guidance message from your mentor.
public class UIMentor : UIPopup {
    public GameObject box;
    public GameObject shadow;
    public GameObject model;
    public UITexture overlay;

    private float overlayAlpha = 0.2f;

    void OnEnable() {
        Show(box);
        Show(shadow);

        overlay.gameObject.SetActive(true);
        StartCoroutine(Fade(overlay, 0f, overlayAlpha));
        StartCoroutine(UIAnimator.BloopUI(model, new Vector3(200, 0, 200), new Vector3(200, 200, 200)));

        // Mentors pause everything.
        Time.timeScale = 0;
    }

    public void Hide() {
        Time.timeScale = 1;
        StartCoroutine(Fade(overlay, overlayAlpha, 0f));
        StartCoroutine(UIAnimator.BloopUI(model, model.transform.localScale, new Vector3(200, 0, 200)));
        base.Hide(box);
        base.Hide(shadow);
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
}
