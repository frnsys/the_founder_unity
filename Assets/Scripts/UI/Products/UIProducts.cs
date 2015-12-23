using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProducts : MonoBehaviour {
    private GameManager gm;

    public GameObject productPrefab;
    public UIGrid grid;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadProducts();
    }

    private void LoadProducts() {
        foreach (ProductRecipe pr in ProductRecipe.LoadAll()) {
            if (pr.name != "Default")
                ShowProduct(pr);
        }
        grid.Reposition();
    }

    private void ShowProduct(ProductRecipe r) {
        GameObject product = NGUITools.AddChild(grid.gameObject, productPrefab);
        product.GetComponent<UIProduct>().recipe = r;
    }
}


