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
        currentScreen_ = manageProducts;
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
        SelectTab(newProduct);
        newProductButton.defaultColor = activeColor;
        manageProductsButton.defaultColor = inactiveColor;
    }

    public void ShowManageProducts() {
        SelectTab(manageProducts);
        newProductButton.defaultColor = inactiveColor;
        manageProductsButton.defaultColor = activeColor;
    }
}
