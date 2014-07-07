using UnityEngine;
using System.Collections;

public class UIDevelopingProducts : MonoBehaviour {
    private GameManager gm;

    public UIGrid grid;
    public GameObject productPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        UpdateDevelopingProducts();
    }

    public void UpdateDevelopingProducts() {
        // Clear the grid.
        while (grid.transform.childCount > 0)
            NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject);

        foreach (Product product in gm.playerCompany.developingProducts) {
            GameObject productItem = NGUITools.AddChild(grid.gameObject, productPrefab);
            productItem.GetComponent<UIProduct>().product = product;
        }

        grid.Reposition();
    }
}


