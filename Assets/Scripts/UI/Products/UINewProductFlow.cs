/*
 * New Product Flow
 * ================
 *
 * Select one or more Product Types
 * to start development of a new product.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    public GameObject productTypePrefab;
    public GameObject selectedProductTypePrefab;
    public GameObject productTypeSelectionView;
    public UIGrid selectedGrid;
    public UIGrid grid;
    public UIButton confirmSelectionButton;

    // Keep track of the selected product aspects.
    private List<ProductType> productTypes;
    private Product product;

    void OnEnable() {
        gm = GameManager.Instance;

        if (productTypes == null) {
            productTypes = new List<ProductType>();
        }

        UpdateConfirmButton();
        LoadProductTypes();
    }

    // Load product types into the grid.
    private void LoadProductTypes() {
        foreach (ProductType pt in ProductType.LoadAll().Where(p => p.isAvailable(gm.playerCompany))) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            UIEventListener.Get(productType).onClick += SelectProductType;
            productType.GetComponent<UIProductType>().productType = pt;
            ToggleProductType(pt, productType);
        }
        foreach (ProductType pt in ProductType.LoadAll().Where(p => !p.isAvailable(gm.playerCompany))) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            productType.GetComponent<UIProductType>().productType = pt;
            productType.transform.Find("Overlay").gameObject.SetActive(true);
            productType.transform.Find("Lock").gameObject.SetActive(true);
        }
        grid.Reposition();
    }

    private void ToggleProductType(ProductType pt, GameObject obj) {
        obj.transform.Find("Overlay").gameObject.SetActive(productTypes.Contains(pt));
    }

    // Select a product type for the new product.
    private void SelectProductType(GameObject obj) {
        ProductType pt = obj.GetComponent<UIProductType>().productType;

        // Deselect
        if (productTypes.Contains(pt)) {
            productTypes.Remove(pt);
            ToggleProductType(pt, obj);
            foreach (Transform t in selectedGrid.transform) {
                if (t.name == pt.name) {
                    NGUITools.Destroy(t.gameObject);
                    break;
                }
            }
            UpdateConfirmButton();
            selectedGrid.Reposition();

        // Required product types is 2.
        } else if (selectedGrid.transform.childCount < 2) {
            GameObject productType = NGUITools.AddChild(selectedGrid.gameObject, selectedProductTypePrefab);
            productType.name = pt.name;
            productType.transform.Find("Label").GetComponent<UILabel>().text = pt.name;
            Transform po = productType.transform.Find("Product Object");
            po.GetComponent<MeshFilter>().mesh = pt.mesh;

            // Deselect on click.
            UIEventListener.Get(productType).onClick += delegate(GameObject go) {
                NGUITools.Destroy(productType);
                productTypes.Remove(pt);
                selectedGrid.Reposition();
                ToggleProductType(pt, obj);
                UpdateConfirmButton();
            };

            productTypes.Add(pt);
            ToggleProductType(pt, obj);
            selectedGrid.Reposition();

            UpdateConfirmButton();
        }
    }

    public void BeginProductDevelopment() {
        UIManager.Instance.Confirm("Are you happy with this product configuration?", BeginProductDevelopment_, null);
    }

    private void BeginProductDevelopment_() {
        //gm.playerCompany.StartNewProduct(productTypes, 0, 0, 0);
        SendMessageUpwards("Close");
    }

    private void UpdateConfirmButton() {
        if (productTypes.Count == 2) {
            confirmSelectionButton.isEnabled = true;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = "Ok, let's get started";
        } else if (productTypes.Count == 0) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select two product types");
        } else if (productTypes.Count == 1) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select one more product type");
        }
    }
}


