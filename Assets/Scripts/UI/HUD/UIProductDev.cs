using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductDev : MonoBehaviour {
    public UILabel designLabel;
    public UILabel engineeringLabel;
    public UILabel marketingLabel;
    public UILabel multiplierLabel;
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

    [SerializeField, HideInInspector]
    private float multiplier;

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
        AudioManager.Instance.PlayLaborFX();
        AddFeature(feature, value);
        AddToCombo(feature, value);
    }

    public void AddFeature(string feature, int value) {
        switch (feature) {
            case "Design":
                AddDesign(value);
                break;
            case "Engineering":
                AddEngineering(value);
                break;
            case "Marketing":
                AddMarketing(value);
                break;
            case "Breakthrough":
                AddDesign(value);
                AddEngineering(value);
                AddMarketing(value);
                break;
        }
    }
    private void AddDesign(int value) {
        design += value;
        SetLabel(designLabel, design);
        StartCoroutine(Pulse(designLabel.gameObject, 1f, 1.6f));
    }
    private void AddEngineering(int value) {
        engineering += value;
        SetLabel(engineeringLabel, engineering);
        StartCoroutine(Pulse(engineeringLabel.gameObject, 1f, 1.6f));
    }
    private void AddMarketing(int value) {
        marketing += value;
        SetLabel(marketingLabel, marketing);
        StartCoroutine(Pulse(marketingLabel.gameObject, 1f, 1.6f));
    }

    private void ClearCombo() {
        int children = comboGrid.transform.childCount;
        for (int i=0;i<children;i++) {
            comboGrid.transform.GetChild(i).gameObject.SetActive(false);
        }
        features.Clear();
        featureValues.Clear();
        multiplier = 1f;
        multiplierLabel.text = "";
    }

    private void ResolveCombo() {
        if (multiplier > 1f)
            AudioManager.Instance.PlayComboFX();

        int count = features.Count(f => f == features[0]);
        float value = (float)(featureValues.Take(count).Average() * multiplier);

        // Bonus
        AddFeature(features[0], (int)value);
        GameManager.Instance.playerCompany.AddPointsToDevelopingProduct(features[0], value);
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

        if ((count > 1 && features[count - 1] != features[count - 2]) || count >= 5) {
            // As soon as a streak is broke or it is of length 5, resolve.
            Invoke("ResolveCombo", 0.25f);
        } else if (count >= 2) {
            // Otherwise, increase the multiplier.
            multiplier += 0.25f;
            multiplierLabel.text = string.Format("{0}x", multiplier);
        }
    }

    private IEnumerator Pulse(GameObject target, float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.1f;

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
