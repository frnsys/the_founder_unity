using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UISelectProductTypes : MonoBehaviour {
    private GameManager gm;
    private List<ProductType> productTypes;
    private int minProductTypes = 5;

    public GameObject productTypePrefab;
    public GameObject selectedProductTypePrefab;
    public UIGrid grid;
    public UIGrid selectedGrid;
    public UIButton confirmSelectionButton;

    void OnEnable() {
        gm = GameManager.Instance;
        productTypes = new List<ProductType>();
        LoadProductTypes();
        UpdateConfirmButton();
    }

    // Load product types into the grid.
    private void LoadProductTypes() {
        foreach (ProductType pt in ProductType.LoadAll().Where(p => gm.playerCompany.productTypes.Contains(p))) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            productType.GetComponent<UIProductType>().productType = pt;
            UIEventListener.Get(productType).onClick += SelectProductType;
        }
        grid.Reposition();
    }

    private void ToggleProductType(ProductType pt, GameObject obj) {
        obj.transform.Find("Overlay").gameObject.SetActive(productTypes.Contains(pt));
    }

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
        } else if (selectedGrid.transform.childCount < minProductTypes) {
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

    private void UpdateConfirmButton() {
        if (productTypes.Count == minProductTypes) {
            confirmSelectionButton.isEnabled = true;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = "Ok, let's get started";
        } else if (productTypes.Count == 0) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select {0} product types", minProductTypes);
        } else if (productTypes.Count == minProductTypes - 1) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select 1 more product type");
        } else {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select {0} more product types", minProductTypes - productTypes.Count);
        }
    }

    public void StartGame() {
        UIManager.Instance.StartGridGame(productTypes);
        SendMessageUpwards("Close");
    }
}


