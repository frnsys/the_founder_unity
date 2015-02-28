using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductDev : MonoBehaviour {
    public UILabel designLabel;
    public UILabel engineeringLabel;
    public UILabel marketingLabel;
    public UIGrid comboGrid;
    public UISprite[] comboSprites;

    [SerializeField, HideInInspector]
    private List<string> features;

    [SerializeField, HideInInspector]
    private List<int> featureValues;

    [SerializeField, HideInInspector]
    private int design;
    [SerializeField, HideInInspector]
    private int engineering;
    [SerializeField, HideInInspector]
    private int marketing;

    public void Clear() {
        design = 0;
        engineering = 0;
        marketing = 0;

        SetLabel(designLabel, 0);
        SetLabel(engineeringLabel, 0);
        SetLabel(marketingLabel, 0);

        ClearCombo();
    }

    public void SetLabel(UILabel label, int value) {
        label.text = string.Format("{0}", value);
    }

    public void Add(string feature, int value) {
        AddFeature(feature, value);
        AddToCombo(feature, value);
    }

    public void AddFeature(string feature, int value) {
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
            case "Marketing":
                marketing += value;
                SetLabel(marketingLabel, marketing);
                StartCoroutine(Pulse(marketingLabel.gameObject, 1f, 1.6f));
                break;
        }
    }

    public void Bonus(string feature) {
        AddFeature(feature, (int)featureValues.Average());
    }

    private void ClearCombo() {
        int children = comboGrid.transform.childCount;
        for (int i=0;i<children;i++) {
            comboGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
        features.Clear();
        featureValues.Clear();
    }

    private void ResolveCombo() {
        // If all are the same.
        if (!features.Any(f => f != features[0])) {
            Bonus(features[0]);
        }

        ClearCombo();
    }

    private void AddToCombo(string feature, int value) {
        features.Add(feature);
        featureValues.Add(value);
        int count = features.Count;

        UISprite sprite = comboSprites[count - 1];
        sprite.spriteName = feature.ToLower();
        sprite.gameObject.SetActive(true);
        StartCoroutine(Pulse(sprite.gameObject, 1f, 1.6f));

        if (count == 3) {
            Invoke("ResolveCombo", 0.25f);
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
