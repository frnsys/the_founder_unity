using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProducts : UIWindow {
    public GameObject newProduct;
    public GameObject manageProducts;
    public UIButton newProductButton;
    public UIButton manageProductsButton;
    public UILabel ppLabel;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    void Update() {
        // TO DO this should show x/y infrastructure available for each infrastructure type.
        //ppLabel.text = GameManager.Instance.playerCompany.availableProductPoints.ToString() + "PP available";
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
