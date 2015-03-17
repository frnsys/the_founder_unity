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
    private List<float> featureValues;

    [SerializeField, HideInInspector]
    private float multiplier;

    private Company company;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        SetLabel(designLabel, company.developingProduct.design.value);
        SetLabel(engineeringLabel, company.developingProduct.engineering.value);
        SetLabel(marketingLabel, company.developingProduct.marketing.value);
    }

    public UIProgressBar productProgressBar;
    void Update() {
        productProgressBar.value = company.developingProduct.progress;
    }

    public void SetLabel(UILabel label, float value) {
        label.text = string.Format("{0:0}", value);
    }

    public void Add(string feature, float value) {
        AudioManager.Instance.PlayLaborFX();
        AddFeature(feature, value);
    }

    public void AddFeature(string feature, float value) {
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
    private void AddDesign(float value) {
        SetLabel(designLabel, company.developingProduct.design.value);
        StartCoroutine(Pulse(designLabel.gameObject, 1f, 1.6f));
    }
    private void AddEngineering(float value) {
        SetLabel(engineeringLabel, company.developingProduct.engineering.value);
        StartCoroutine(Pulse(engineeringLabel.gameObject, 1f, 1.6f));
    }
    private void AddMarketing(float value) {
        SetLabel(marketingLabel, company.developingProduct.marketing.value);
        StartCoroutine(Pulse(marketingLabel.gameObject, 1f, 1.6f));
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
