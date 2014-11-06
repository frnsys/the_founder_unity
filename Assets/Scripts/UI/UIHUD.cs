using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Displays info about short(er) term events,
// such as progress and revenue from products
// and research progress.
public class UIHUD : MonoBehaviour {
    private GameManager gm;

    public GameObject activeProductPrefab;
    private GameObject productsIndicator;
    private GameObject developmentIndicator;
    private GameObject researchIndicator;
    private GameObject productsGrid;
    private UIProgressBar researchProgressBar;
    private UIProgressBar developmentProgressBar;

    void OnEnable() {
        gm = GameManager.Instance;

        productsIndicator      = transform.Find("Products").gameObject;
        developmentIndicator   = transform.Find("Development").gameObject;
        researchIndicator      = transform.Find("Research").gameObject;
        researchProgressBar    = researchIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
        developmentProgressBar = developmentIndicator.transform.Find("Indicator/Progress Bar").gameObject.GetComponent<UIProgressBar>();
        productsGrid           = productsIndicator.transform.Find("Indicator").gameObject;
    }

    // Keep track of which products are already displayed so we don't create redundant ones.
    private List<Product> displayedProducts = new List<Product>();

    void Update() {
        Vector3 pos = Vector3.zero;
        float height = 0;

        // In-market product info.
        if (gm.playerCompany.activeProducts.Count > 0) {
            productsIndicator.SetActive(true);
            productsIndicator.transform.localPosition = pos;

            foreach (Product p in gm.playerCompany.activeProducts) {
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


        // Developing product info.
        if (gm.playerCompany.developingProducts.Count > 0) {
            developmentIndicator.SetActive(true);
            developmentIndicator.transform.localPosition = pos;

            // TO DO this should create a progress bar for each developing product.
            developmentProgressBar.value = gm.playerCompany.developingProducts[0].progress;
            height = developmentIndicator.GetComponent<UIWidget>().CalculateBounds().size.y;
            pos.y += height;
        } else {
            developmentIndicator.SetActive(false);
        }


        // Research info.
        if (gm.researchManager.researching) {
            researchIndicator.SetActive(true);
            researchIndicator.transform.localPosition = pos;
            researchProgressBar.value = gm.researchManager.progress;
        } else {
            researchIndicator.SetActive(false);
        }
    }
}


