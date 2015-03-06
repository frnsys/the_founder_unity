using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Displays info about short(er) term events,
// such as progress and revenue from products.
public class UIHUD : MonoBehaviour {
    private GameManager gm;
    private Company company;

    public UIGrid grid;
    public GameObject activeProductPrefab;
    public Color highlightColor;

    public GameObject promoIndicator;
    public GameObject productIndicator;
    public GameObject recruitingIndicator;
    public GameObject specialProjectIndicator;

    public UIProgressBar promoProgressBar;
    public UIProgressBar productProgressBar;
    public UIProgressBar recruitingProgressBar;
    public UIProgressBar specialProjectProgressBar;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        StartCoroutine(UpdateHUD());
    }

    // Keep track of which products are already displayed so we don't create redundant ones.
    // Cache other stuff too so we don't need to constantly look for it.
    private List<Product> displayedProducts = new List<Product>();
    private List<GameObject> productObjects = new List<GameObject>();
    private List<UILabel> revenueLabels = new List<UILabel>();
    private List<UILabel> marketLabels = new List<UILabel>();

    IEnumerator UpdateHUD() {
        while (true) {
            for (int i=0; i < displayedProducts.Count; i++) {
                // Cleanup shutdown products.
                if (displayedProducts[i].retired) {
                    NGUITools.Destroy(productObjects[i]);
                    revenueLabels.RemoveAt(i);
                    marketLabels.RemoveAt(i);
                    productObjects.RemoveAt(i);
                    displayedProducts.RemoveAt(i);

                // Update revenue & market share label otherwise.
                } else {
                    revenueLabels[i].text = string.Format("{0:C0}", displayedProducts[i].revenueEarned);
                    marketLabels[i].text = string.Format("{0:0}%", Mathf.Ceil(displayedProducts[i].marketShare));
                }
            }

            // Show in-market products.
            foreach (Product p in gm.playerCompany.activeProducts) {
                if (!displayedProducts.Contains(p)) {
                    GameObject productItem = NGUITools.AddChild(grid.gameObject, activeProductPrefab);
                    UILabel nameLabel = productItem.transform.Find("Name").GetComponent<UILabel>();
                    if (p.synergy)
                        nameLabel.color = highlightColor;
                    nameLabel.text = p.name;

                    UILabel revenueLabel = productItem.transform.Find("Revenue").GetComponent<UILabel>();
                    revenueLabel.text = string.Format("{0:C0}", p.revenueEarned);

                    UILabel marketLabel = productItem.transform.Find("Market Share").GetComponent<UILabel>();
                    marketLabel.text = string.Format("{0:0}%", Mathf.Ceil(p.marketShare));

                    displayedProducts.Add(p);
                    productObjects.Add(productItem);
                    revenueLabels.Add(revenueLabel);
                    marketLabels.Add(marketLabel);;
                }
            }

            // Developing product info.
            if (company.developingProduct != null) {
                productIndicator.SetActive(true);
                productProgressBar.value = company.developingProduct.progress;
            } else {
                productIndicator.SetActive(false);
            }

            // Developing special project info.
            if (company.developingSpecialProject != null) {
                specialProjectIndicator.SetActive(true);
                specialProjectProgressBar.value = company.developingSpecialProject.progress;
            } else {
                specialProjectIndicator.SetActive(false);
            }

            // Promo info.
            if (company.developingPromo != null) {
                promoIndicator.SetActive(true);
                promoProgressBar.value = company.developingPromo.progress;
            } else {
                promoIndicator.SetActive(false);
            }

            // Recruiting info.
            if (company.developingRecruitment != null) {
                recruitingIndicator.SetActive(true);
                recruitingProgressBar.value = company.developingRecruitment.progress;
            } else {
                recruitingIndicator.SetActive(false);
            }

            grid.Reposition();
            yield return new WaitForSeconds(0.2f);
        }
    }
}


