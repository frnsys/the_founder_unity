using UnityEngine;
using System;
using System.Collections;

public class UILabor : MonoBehaviour {
    public UISprite icon;
    public UILabel amount;
    public UIProgressBar timer;

    private Stat stat_;
    public Stat stat {
        get {
            return stat_;
        }
        set {
            stat_ = value;
            icon.spriteName = stat_.name.ToLower();
            amount.text = string.Format("+{0:F0}", stat_.value);
            timer.value = 1f;
        }
    }

    void OnEnable() {
        StartCoroutine(Scale(0f, 1f));
    }

    private IEnumerator Scale(float from, float to, Action<GameObject> cb = null) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.05f;

        for (float f = 0f; f <= 1f + step; f += step) {
            transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        if (cb != null) {
            cb(gameObject);
        }
    }

    void Update() {
        timer.value -= 0.008f;

        if (timer.value <= 0)
            StartCoroutine(Scale(1f, 0f, Destroy));
    }

    public void Capture() {
        if (stat_.name == "Breakthrough") {
            GameManager.Instance.playerCompany.AddPointsToDevelopingProduct("Design", stat_.value);
            UIManager.Instance.AddPointsToDevelopingProduct("Design", stat_.value);

            GameManager.Instance.playerCompany.AddPointsToDevelopingProduct("Engineering", stat_.value);
            UIManager.Instance.AddPointsToDevelopingProduct("Engineering", stat_.value);

            GameManager.Instance.playerCompany.AddPointsToDevelopingProduct("Marketing", stat_.value);
            UIManager.Instance.AddPointsToDevelopingProduct("Marketing", stat_.value);
        } else {
            GameManager.Instance.playerCompany.AddPointsToDevelopingProduct(stat_.name, stat_.value);
            UIManager.Instance.AddPointsToDevelopingProduct(stat_.name, stat_.value);
        }
        Destroy(gameObject);
    }
}
