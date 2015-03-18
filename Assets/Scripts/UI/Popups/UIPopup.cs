using UnityEngine;
using System;
using System.Collections;

// Popup class with scaling up/down show/hide behavior (respectively).
public abstract class UIPopup : MonoBehaviour {
    public void Show(GameObject target, float from=0f, float to=1f) {
        PreSetup(gameObject);
        StartCoroutine(Scale(target, from, to, Setup));
    }

    public void Hide(GameObject target, float from=1f, float to=0f) {
        StartCoroutine(Scale(target, from, to, NGUITools.DestroyImmediate));
    }

    // Override this to set stuff up before scaling up.
    protected virtual void PreSetup(GameObject obj) {}

    // Override this to set stuff up after scaling up.
    protected virtual void Setup(GameObject obj) {}

    protected IEnumerator Scale(GameObject target, float from, float to, Action<GameObject> cb = null) {
        return UIAnimator.ScaleUI(target, from, to, delegate() {
            cb(gameObject);
        });
    }

    protected IEnumerator Fade(UITexture texture, float from, float to) {
        return UIAnimator.FadeUI(texture, from, to);
    }
}
