using UnityEngine;
using System;
using System.Collections;

public class UIAnimator : MonoBehaviour {

    public static void Rotate(GameObject obj) {
        obj.transform.Rotate(0, 0, -50 * Time.deltaTime);
    }

    public static IEnumerator Pulse(Transform t, float from, float to, float step) {
        Vector3 fromScale = new Vector3(from, from ,from);
        Vector3 toScale = new Vector3(to, to, to);

        while (true) {
            for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
                t.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }

            for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
                t.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
        }
    }

    public static IEnumerator Scale(Transform t, float from, float to, float step) {
        Vector3 fromScale = new Vector3(from, from, from);
        Vector3 toScale = new Vector3(to,to,to);

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            t.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    public static IEnumerator Bloop(Transform t, float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        Vector3 toScale_ = toScale * 1.2f;
        float step = 2f;

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            t.localScale = Vector3.Lerp(fromScale, toScale_, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        step = 24f;
        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            t.localScale = Vector3.Lerp(toScale_, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    public static IEnumerator Raise(Transform t, float y, float step) {
        Vector3 fromPos = t.localPosition;
        Vector3 toPos = t.localPosition;
        toPos.y = y;

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            t.localPosition = Vector3.Lerp(fromPos, toPos, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    // The UI flavors of animations are not sensitive to Time.timeScale.
    public static IEnumerator PulseUI(Transform t, float from, float to, float step) {
        Vector3 fromScale = new Vector3(from, from ,from);
        Vector3 toScale = new Vector3(to, to, to);

        TimeKeep time = new TimeKeep(0);
        while (true) {
            for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
                time = UpdateTime(time);
                t.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }

            for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
                time = UpdateTime(time);
                t.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
        }
    }

    public static IEnumerator ScaleUI(GameObject target, float from, float to, Action cb = null) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 6f;

        TimeKeep time = new TimeKeep(0);
        for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
            time = UpdateTime(time);

            // SmoothStep gives us a bit of easing.
            target.transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        if (cb != null)
            cb();
    }

    public static IEnumerator FadeUI(UITexture texture, float from, float to) {
        float step = 4f;
        TimeKeep time = new TimeKeep(0);
        for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
            time = UpdateTime(time);
            texture.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }

    public static IEnumerator BloopUI(GameObject obj, Vector3 from, Vector3 to) {
        // Scale the model up and down, with a little bounciness.
        float step = 10f;
        Vector3 to_ = to * 1.2f;
        TimeKeep time = new TimeKeep(0);
        for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
            time = UpdateTime(time);
            obj.transform.localScale = Vector3.Lerp(from, to_, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        step = 30f;
        for (float f = 0f; f <= 1f + step * time.diff; f += step * time.diff) {
            time = UpdateTime(time);
            obj.transform.localScale = Vector3.Lerp(to_, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }


    // Custom deltaTime stuff.
    public struct TimeKeep {
        public float last;
        public float diff;

        public TimeKeep(float _) {
            last = Time.realtimeSinceStartup;
            diff = Time.deltaTime;
        }

    }
    public static TimeKeep UpdateTime(TimeKeep t) {
        float total = Time.realtimeSinceStartup;
        t.diff = total - t.last;
        t.last = total;

        if (t.diff == 0)
            t.diff = Time.deltaTime;

        return t;
    }
}
