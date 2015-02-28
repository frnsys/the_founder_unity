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
    public List<GameObject> productTypeItems;
    public UIGrid selectedGrid;
    public UIGrid grid;
    public UIButton confirmSelectionButton;

    public GameObject blackout;
    public UIProgressBar progressBar;

    // Keep track of the selected product aspects.
    private List<ProductType> productTypes;
    private Product product;

    void OnEnable() {
        gm = GameManager.Instance;

        if (productTypes == null) {
            productTypes = new List<ProductType>();
            productTypeItems = new List<GameObject>();
        }

        if (productTypes.Count == 0) {
            confirmSelectionButton.isEnabled = false;
        } else {
            confirmSelectionButton.isEnabled = true;
        }

        LoadProductTypes();
    }

    void Update() {
        // Prevent access if something is already developing.
        if (gm.playerCompany.developing) {
            blackout.SetActive(true);

            if (gm.playerCompany.developingProduct != null) {
                progressBar.value = gm.playerCompany.developingProduct.progress;
            } else {
                progressBar.gameObject.SetActive(false);
            }

        } else {
            blackout.SetActive(false);
        }
    }

    // Load product types into the grid.
    private void LoadProductTypes() {
        productTypeItems.Clear();
        foreach (ProductType pt in gm.unlocked.productTypes) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            UIEventListener.Get(productType).onClick += SelectProductType;
            productType.GetComponent<UIProductType>().productType = pt;
            productTypeItems.Add(productType);

            bool capacity = HasCapacityFor(pt);
            productType.GetComponent<UIButton>().isEnabled = capacity;
            productType.transform.Find("Overlay").gameObject.SetActive(!capacity);
        }
        grid.Reposition();
    }

    // Select a product type for the new product.
    private void SelectProductType(GameObject obj) {
        // Required product types is 2.
        if (selectedGrid.transform.childCount < 2) {
            ProductType pt = obj.GetComponent<UIProductType>().productType;
            GameObject productType = NGUITools.AddChild(selectedGrid.gameObject, selectedProductTypePrefab);

            productType.transform.Find("Label").GetComponent<UILabel>().text = pt.name;
            Transform po = productType.transform.Find("Product Object");
            po.GetComponent<MeshFilter>().mesh = pt.mesh;

            UIEventListener.Get(productType.transform.Find("Remove").gameObject).onClick += delegate(GameObject go) {
                NGUITools.Destroy(productType);
                productTypes.Remove(pt);
                selectedGrid.Reposition();

                if (productTypes.Count != 2)
                    confirmSelectionButton.isEnabled = false;

                UpdateProductTypeItems();
            };

            productTypes.Add(pt);
            selectedGrid.Reposition();

            if (productTypes.Count == 2)
                confirmSelectionButton.isEnabled = true;

            UpdateProductTypeItems();
        }
    }

    // Update which product types can be selected.
    private void UpdateProductTypeItems() {
        foreach (GameObject item in productTypeItems) {
            ProductType pt = item.GetComponent<UIProductType>().productType;

            bool capacity = HasCapacityFor(pt);
            item.GetComponent<UIButton>().isEnabled = capacity;
            item.transform.Find("Overlay").gameObject.SetActive(!capacity);
        }
    }

    // Check if the company has enough capacity for a particular product type.
    private bool HasCapacityFor(ProductType pt) {
        Infrastructure selectionInf = new Infrastructure();
        if (productTypes.Count > 0) {
            IEnumerable<Infrastructure> selectionInfs = productTypes.Select(x => x.requiredInfrastructure);
            if (selectionInfs.Count() > 0)
                selectionInf += selectionInfs.Aggregate((x, y) => x + y);
        }
        return (gm.playerCompany.availableInfrastructure - selectionInf) >= pt.requiredInfrastructure;
    }

    // Go to the point allocation page.
    public void ConfirmSelection() {
        // Not very elegant, but ping the narrative manager
        // to see if anything needs to be displayed.
        gm.narrativeManager.ProgressOnboarding();
    }

    public void BeginProductDevelopment() {
        UIManager.Instance.Confirm("Are you happy with this product configuration?", BeginProductDevelopment_, null);
    }

    private void BeginProductDevelopment_() {
        gm.playerCompany.StartNewProduct(productTypes, 0, 0, 0);
        SendMessageUpwards("Close");

        // Not very elegant, but ping the narrative manager
        // to see if anything needs to be displayed.
        gm.narrativeManager.ProgressOnboarding();
    }
}


