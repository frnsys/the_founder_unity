using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductTypes : MonoBehaviour {
    private GameManager gm;

    public GameObject productTypePrefab;
    public UIGrid grid;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadProductTypes();
    }

    // Load product types into the grid.
    private void LoadProductTypes() {
       foreach (ProductType pt in ProductType.LoadAll().Where(p => gm.playerCompany.productTypes.Contains(p))) {
            ShowProductType(pt, false);
        }
        foreach (ProductType pt in ProductType.LoadAll().Where(p => !gm.playerCompany.productTypes.Contains(p)
                    && p.isAvailable(gm.playerCompany))) {
            ShowProductType(pt, true);
        }
        foreach (ProductType pt in ProductType.LoadAll().Where(p => !gm.playerCompany.productTypes.Contains(p)
                    && !p.isAvailable(gm.playerCompany))) {
            ShowProductType(pt, true);
        }
        grid.Reposition();
    }

    private void ShowProductType(ProductType pt, bool locked) {
        GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
        productType.GetComponent<UIProductType>().productType = pt;
        if (locked)
            productType.GetComponent<UIProductType>().Lock();
        else
            productType.GetComponent<UIProductType>().Unlock();
    }
}


