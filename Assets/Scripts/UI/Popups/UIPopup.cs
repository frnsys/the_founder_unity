using UnityEngine;
using System;
using System.Collections;

// Popup class with scaling up/down show/hide behavior (respectively).
public abstract class UIPopup : MonoBehaviour {
    public void Show(GameObject target) {
        StartCoroutine(Scale(target, 0f, 1f));
    }

    public void Hide(GameObject target) {
        StartCoroutine(Scale(target, 1f, 0f, NGUITools.DestroyImmediate));
    }

    private IEnumerator Scale(GameObject target, float from, float to, Action<GameObject> cb = null) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.1f;

        for (float f = 0f; f <= 1f + step; f += step) {
            // SmoothStep gives us a bit of easing.
            target.transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        if (cb != null) {
            cb(gameObject);
        }
    }
}
