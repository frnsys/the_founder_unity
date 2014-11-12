using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManageProducts : MonoBehaviour {
    public GameObject productItemPrefab;
    public GameObject productsGrid;
    public UICenteredGrid productsGridGrid;

    void OnEnable() {
        productsGridGrid.fullWidth = true;
    }

    private List<Product> displayedProducts = new List<Product>();
    void Update() {
        foreach (Product p in GameManager.Instance.playerCompany.products) {
            // Show only developing and in-market products.
            if (!p.retired) {
                if (!displayedProducts.Contains(p)) {
                    GameObject productItem = NGUITools.AddChild(productsGrid, productItemPrefab);
                    UIProduct uip = productItem.GetComponent<UIProduct>();

                    uip.product = p;

                    if (p.developing) {
                        uip.revenue.gameObject.SetActive(false);
                        uip.progress.gameObject.SetActive(true);
                    }

                    UIEventListener.Get(productItem.transform.Find("Shutdown Button").gameObject).onClick += ShutdownProduct;

                    displayedProducts.Add(p);

                } else {
                    // Just update the revenue.
                    int i = displayedProducts.IndexOf(p);
                    UIProduct uip = productsGrid.transform.GetChild(i).GetComponent<UIProduct>();

                    if (p.state == Product.State.LAUNCHED) {
                        uip.revenue.text = "$" + ((int)p.revenueEarned).ToString();
                        uip.revenue.gameObject.SetActive(true);
                        uip.progress.gameObject.SetActive(false);
                    } else if (p.state == Product.State.DEVELOPMENT) {
                        uip.progress.value = p.progress;
                    }
                }
            }
        }
        productsGridGrid.Reposition();
    }

    public void ShutdownProduct(GameObject obj) {
        GameObject productItem = obj.transform.parent.gameObject;
        Product product = productItem.GetComponent<UIProduct>().product;

        Action yes = delegate() {
            product.Shutdown();
            displayedProducts.Remove(product);
            UnityEngine.Object.DestroyImmediate(productItem);
            productsGridGrid.Reposition();
        };
        UIManager.Instance.Confirm("Are you sure want to shutdown " + product.name + "?", yes, null);
    }

    void OnDisable() {
        foreach (Transform child in productsGrid.transform) {
            UIEventListener.Get(child.Find("Shutdown Button").gameObject).onClick -= ShutdownProduct;
        }
    }
}
