using UnityEngine;
using System.Collections;

public class UIHUD : MonoBehaviour {
    private GameManager gm;

    private GameObject productsIndicator;
    private GameObject developmentIndicator;
    private GameObject researchIndicator;
    private UIProgressBar researchProgressBar;

    void OnEnable() {
        gm = GameManager.Instance;

        productsIndicator = transform.Find("Products").gameObject;
        developmentIndicator = transform.Find("Development").gameObject;
        researchIndicator = transform.Find("Research").gameObject;
        researchProgressBar = researchIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
    }

    void Update() {
        Vector3 pos = Vector3.zero;
        float height = 0;

        // temp: this should check if there are any active products
        if (false) {
            productsIndicator.SetActive(true);
            productsIndicator.transform.localPosition = pos;
            height = productsIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            productsIndicator.SetActive(false);
        }

        // temp: this should check if a product is being developed
        if (false) {
            developmentIndicator.SetActive(true);
            developmentIndicator.transform.localPosition = pos;
            height = developmentIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            developmentIndicator.SetActive(false);
        }

        if (GameManager.Instance.playerCompany.researching) {
            researchIndicator.SetActive(true);
            Debug.Log(pos);
            researchIndicator.transform.localPosition = pos;
            researchProgressBar.value = GameManager.Instance.playerCompany.consultancy.researchProgress;
        } else {
            researchIndicator.SetActive(false);
        }
    }
}


