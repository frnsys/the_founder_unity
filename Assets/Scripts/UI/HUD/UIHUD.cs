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

    public UIProgressBar productDevelopment;
    public GameObject newProductButton;
    public UILabel devEngineering;
    public UILabel devDesign;
    public UILabel devMarketing;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        StartCoroutine(UpdateHUD());

        Company.BeganProduct += BeganProduct;
        Product.Completed += CompletedProduct;
    }

    // Keep track of which products are already displayed so we don't create redundant ones.
    // Cache other stuff too so we don't need to constantly look for it.
    private List<Product> displayedProducts = new List<Product>();
    private List<GameObject> productObjects = new List<GameObject>();
    private List<UILabel> revenueLabels = new List<UILabel>();

    IEnumerator UpdateHUD() {
        while (true) {
            for (int i=0; i < displayedProducts.Count; i++) {
                // Cleanup shutdown products.
                if (displayedProducts[i].retired) {
                    NGUITools.Destroy(productObjects[i]);
                    revenueLabels.RemoveAt(i);
                    productObjects.RemoveAt(i);
                    displayedProducts.RemoveAt(i);

                // Update revenue label otherwise.
                } else {
                    revenueLabels[i].text = string.Format("{0:C0}", displayedProducts[i].revenueEarned);
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

                    displayedProducts.Add(p);
                    productObjects.Add(productItem);
                    revenueLabels.Add(revenueLabel);
                }
            }

            if (company.developing) {
                productDevelopment.value = company.developingProduct.progress;
                devEngineering.text = string.Format("{0:F0}", company.developingProduct.engineering.baseValue);
                devDesign.text = string.Format("{0:F0}", company.developingProduct.design.baseValue);
                devMarketing.text = string.Format("{0:F0}", company.developingProduct.marketing.baseValue);
            }

            grid.Reposition();
            yield return StartCoroutine(GameTimer.Wait(0.2f));
        }
    }


    void BeganProduct(Product p, Company c) {
        productDevelopment.value = 0;
        productDevelopment.gameObject.SetActive(true);
        newProductButton.SetActive(false);
    }

    void CompletedProduct(Product p, Company c) {
        productDevelopment.gameObject.SetActive(false);
        newProductButton.SetActive(true);
    }
}


