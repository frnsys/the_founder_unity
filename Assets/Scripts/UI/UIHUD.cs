using UnityEngine;
using System.Collections;

public class UIHUD : MonoBehaviour {
    private GameManager gm;

    private GameObject productsIndicator;
    private GameObject developmentIndicator;
    private GameObject researchIndicator;
    private UIProgressBar researchProgressBar;
    private UIProgressBar developmentProgressBar;

    void OnEnable() {
        gm = GameManager.Instance;

        productsIndicator = transform.Find("Products").gameObject;
        developmentIndicator = transform.Find("Development").gameObject;
        researchIndicator = transform.Find("Research").gameObject;
        researchProgressBar = researchIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
        developmentProgressBar = developmentIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
    }

    void Update() {
        Vector3 pos = Vector3.zero;
        float height = 0;

        if (false) {
            productsIndicator.SetActive(true);
            productsIndicator.transform.localPosition = pos;
            height = productsIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            productsIndicator.SetActive(false);
        }

        // temp: this should check if a product is being developed
        if (GameManager.Instance.playerCompany.developingProducts.Count > 0) {
            developmentIndicator.SetActive(true);
            developmentIndicator.transform.localPosition = pos;
            // TO DO this should create a progress bar for each developing product. Or we should just limit it to one product in development.
            developmentProgressBar.value = GameManager.Instance.playerCompany.developingProducts[0].progress;
            height = developmentIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            developmentIndicator.SetActive(false);
        }

        if (GameManager.Instance.researchManager.researching) {
            researchIndicator.SetActive(true);
            researchIndicator.transform.localPosition = pos;
            researchProgressBar.value = GameManager.Instance.researchManager.progress;
        } else {
            researchIndicator.SetActive(false);
        }
    }
}


