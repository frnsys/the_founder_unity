using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProducts : UIWindow {
    public GameObject newProduct;
    public GameObject manageProducts;
    public UIButton newProductButton;
    public UIButton manageProductsButton;
    public UILabel infrastructureLabel;

    private Company playerCompany;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    void Awake() {
        playerCompany = GameManager.Instance.playerCompany;
    }

    protected override void Setup(GameObject obj) {
        GameManager.Instance.narrativeManager.OpenedProducts();
    }

    void Update() {
        Infrastructure infra = playerCompany.infrastructure;
        Infrastructure used = playerCompany.usedInfrastructure;

        // Update the status of used vs usedable infrastructure.
        string infStatus = "Used Infrastructure => ";
        foreach (Infrastructure.Type t in Infrastructure.Types) {
            infStatus += t.ToString() + ": " + used[t].ToString() + "/" + infra[t].ToString() + " ";
        }
        infrastructureLabel.text = infStatus;
    }

    public void ShowNewProductFlow() {
        newProduct.SetActive(true);
        newProductButton.defaultColor = activeColor;

        manageProducts.SetActive(false);
        manageProductsButton.defaultColor = inactiveColor;
    }

    public void ShowManageProducts() {
        newProduct.SetActive(false);
        newProductButton.defaultColor = inactiveColor;

        manageProducts.SetActive(true);
        manageProductsButton.defaultColor = activeColor;
    }
}
