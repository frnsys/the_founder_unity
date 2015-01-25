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
        // Update the status of used/available infrastructure.
        Infrastructure infra = playerCompany.infrastructure;
        string infStatus;
        if (currentScreen_ == manageProducts) {
            infStatus = "Used INF :: ";
            foreach (Infrastructure.Type t in Infrastructure.Types) {
                infStatus += t.ToString() + ": " + playerCompany.usedInfrastructure[t].ToString() + "/" + infra[t].ToString() + " ";
            }
        } else {
            infStatus = "Available INF :: ";
            foreach (Infrastructure.Type t in Infrastructure.Types) {
                infStatus += t.ToString() + ": " + playerCompany.availableInfrastructure[t].ToString() + " ";
            }
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
