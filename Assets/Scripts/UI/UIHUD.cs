using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHUD : MonoBehaviour {
    private GameManager gm;

    private GameObject productsIndicator;
    private GameObject developmentIndicator;
    private GameObject researchIndicator;
    private GameObject productsGrid;
    private UIProgressBar researchProgressBar;
    private UIProgressBar developmentProgressBar;
    public GameObject activeProductPrefab;

    void OnEnable() {
        gm = GameManager.Instance;

        productsIndicator = transform.Find("Products").gameObject;
        developmentIndicator = transform.Find("Development").gameObject;
        researchIndicator = transform.Find("Research").gameObject;
        researchProgressBar = researchIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
        developmentProgressBar = developmentIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
        productsGrid = productsIndicator.transform.Find("Indicator").gameObject;
    }

    private List<Product> displayedProducts = new List<Product>();

    void Update() {
        Vector3 pos = Vector3.zero;
        float height = 0;

        if (GameManager.Instance.playerCompany.activeProducts.Count > 0) {
            productsIndicator.SetActive(true);
            productsIndicator.transform.localPosition = pos;

            foreach (Product p in GameManager.Instance.playerCompany.activeProducts) {
                if (!displayedProducts.Contains(p)) {
                    GameObject productItem = NGUITools.AddChild(productsGrid, activeProductPrefab);
                    productItem.transform.Find("Product Name").GetComponent<UILabel>().text = p.name;
                    productItem.transform.Find("Product Revenue").GetComponent<UILabel>().text = "$" + p.revenueEarned.ToString();
                    displayedProducts.Add(p);
                } else {
                    int i = displayedProducts.IndexOf(p);
                    productsGrid.transform.GetChild(i).Find("Product Revenue").GetComponent<UILabel>().text = "$" + ((int)p.revenueEarned).ToString();
                }
            }

            productsGrid.GetComponent<UICenteredGrid>().Reposition();
            height = productsIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            productsIndicator.SetActive(false);
        }

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


