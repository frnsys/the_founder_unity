using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProductDev : MonoBehaviour {
    public UILabel designLabel;
    public UILabel engineeringLabel;
    public UILabel marketingLabel;

    [HideInInspector]
    public int design;
    [HideInInspector]
    public int engineering;
    [HideInInspector]
    public int marketing;

    public void Clear() {
        design = 0;
        engineering = 0;
        marketing = 0;

        SetLabel(designLabel, 0);
        SetLabel(engineeringLabel, 0);
        SetLabel(marketingLabel, 0);
    }

    public void SetLabel(UILabel label, int value) {
        label.text = string.Format("{0}", value);
    }

    public void Add(string feature, int value) {
        switch (feature) {
            case "Design":
                design += value;
                SetLabel(designLabel, design);
                StartCoroutine(Pulse(designLabel.gameObject, 1f, 1.6f));
                break;
            case "Engineering":
                engineering += value;
                SetLabel(engineeringLabel, engineering);
                StartCoroutine(Pulse(engineeringLabel.gameObject, 1f, 1.6f));
                break;
                break;
            case "Marketing":
                marketing += value;
                SetLabel(marketingLabel, marketing);
                StartCoroutine(Pulse(marketingLabel.gameObject, 1f, 1.6f));
                break;
        }
    }


    private IEnumerator Pulse(GameObject target, float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.05f;

        for (float f = 0f; f <= 1f + step; f += step) {
            target.transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        for (float f = 0f; f <= 1f + step; f += step) {
            target.transform.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

    }
}
