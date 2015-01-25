using UnityEngine;
using System;
using System.Linq;
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
            // If we aren't already displaying this product, display it.
            if (!displayedProducts.Contains(p)) {
                GameObject productItem = NGUITools.AddChild(productsGrid, productItemPrefab);
                UIProduct uip = productItem.GetComponent<UIProduct>();
                uip.product = p;

                UIEventListener.Get(productItem.transform.Find("Shutdown Button").gameObject).onClick += ShutdownProduct;

                displayedProducts.Add(p);
            }

            int childIdx = displayedProducts.IndexOf(p);
            productsGrid.transform.GetChild(childIdx).Find("Disabled").gameObject.SetActive(p.disabled);
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

        if (product.developing)
            UIManager.Instance.Confirm("Are you sure want to stop developing this " + product.genericName + "?", yes, null);
        else
            UIManager.Instance.Confirm("Are you sure want to stop shutdown " + product.name + "? Its effects will become inactive.", yes, null);
    }
}
