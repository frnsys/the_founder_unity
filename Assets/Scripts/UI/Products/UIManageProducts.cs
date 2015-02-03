using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageProducts : UIFullScreenPager {
    private Company company;
    private GameManager gm;
    public GameObject productItemPrefab;


    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
    }

    private List<Product> displayedProducts = new List<Product>();
    void Update() {
        foreach (Product p in company.products) {
            // If we aren't already displaying this product, display it.
            if (!displayedProducts.Contains(p)) {
                GameObject productItem = NGUITools.AddChild(grid.gameObject, productItemPrefab);
                UIProduct uip = productItem.GetComponent<UIProduct>();
                uip.product = p;

                UIEventListener.Get(productItem.transform.Find("Shutdown Button").gameObject).onClick += ShutdownProduct;

                displayedProducts.Add(p);
            }

            int childIdx = displayedProducts.IndexOf(p);
            grid.transform.GetChild(childIdx).Find("Disabled").gameObject.SetActive(p.disabled);
        }
        Adjust();
    }

    public void ShutdownProduct(GameObject obj) {
        GameObject productItem = obj.transform.parent.gameObject;
        Product product = productItem.GetComponent<UIProduct>().product;

        Action yes = delegate() {
            product.Shutdown();
            displayedProducts.Remove(product);
            UnityEngine.Object.DestroyImmediate(productItem);
            Adjust();
        };

        if (product.developing)
            UIManager.Instance.Confirm("Are you sure want to stop developing this " + product.genericName + "?", yes, null);
        else
            UIManager.Instance.Confirm("Are you sure want to stop shutdown " + product.name + "? Its effects will become inactive.", yes, null);
    }
}
