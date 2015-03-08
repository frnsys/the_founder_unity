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
        float step = 0.1f;

        for (float f = 0f; f <= 1f + step; f += step) {
            transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        if (cb != null) {
            cb(gameObject);
        }
    }

    void Update() {
        timer.value -= 0.02f;

        if (timer.value <= 0 && !captured)
            StartCoroutine(Scale(1f, 0f, Destroy));
    }

    void OnClick() {
        if (!captured) {
            captured = true;
            GameManager.Instance.playerCompany.AddPointsToDevelopingProduct(stat_.name, stat_.value);
            UIManager.Instance.AddPointsToDevelopingProduct(stat_.name, stat_.value);
            StartCoroutine(Pulse(gameObject, 1f, 1.4f));
        }
    }

    [SerializeField, HideInInspector]
    private bool captured = false;
    private IEnumerator Pulse(GameObject target, float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.15f;

        for (float f = 0f; f <= 1f + step; f += step) {
            target.transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        for (float f = 0f; f <= 1f + step; f += step) {
            target.transform.localScale = Vector3.Lerp(toScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        Destroy(gameObject);
    }
}
