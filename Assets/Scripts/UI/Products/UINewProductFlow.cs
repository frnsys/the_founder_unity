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
    public GameObject pointAllocationView;
    public List<GameObject> productTypeItems;
    public UIGrid selectedGrid;
    public UISimpleGrid grid;
    public UIButton confirmSelectionButton;

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

    // Load product types into the grid.
    private void LoadProductTypes() {
        grid.Clear();
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
        if (selectedGrid.transform.childCount < 2) {
            confirmSelectionButton.isEnabled = true;
            ProductType pt = obj.GetComponent<UIProductType>().productType;
            GameObject productType = NGUITools.AddChild(selectedGrid.gameObject, selectedProductTypePrefab);

            productType.transform.Find("Label").GetComponent<UILabel>().text = pt.name;
            UIEventListener.Get(productType.transform.Find("Remove").gameObject).onClick += delegate(GameObject go) {
                NGUITools.Destroy(productType);
                productTypes.Remove(pt);
                selectedGrid.Reposition();

                if (productTypes.Count == 0)
                    confirmSelectionButton.isEnabled = false;

                UpdateProductTypeItems();
            };

            productTypes.Add(pt);
            selectedGrid.Reposition();
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
        productTypeSelectionView.SetActive(false);
        pointAllocationView.SetActive(true);

        // Create a dummy product with the new product types.
        product = new Product();
        product.Init(productTypes, 0, 0, 0);

        // Update the required progress for each feature.
        GameObject obj;
        obj = transform.Find("Point Allocation/Features/Design").gameObject;
        UpdateProgressRequired(obj, "Design",       design);

        obj = transform.Find("Point Allocation/Features/Marketing").gameObject;
        UpdateProgressRequired(obj, "Marketing",    marketing);

        obj = transform.Find("Point Allocation/Features/Engineering").gameObject;
        UpdateProgressRequired(obj, "Engineering",  engineering);
    }

    // ===============================================
    // Point Allocation ==============================
    // ===============================================

    private int design = 0;
    private int marketing = 0;
    private int engineering = 0;
    private Color activeColor = new Color(1f, 0.49f, 0.49f, 1f);
    private Color inactiveColor = new Color(1f, 0.76f, 0.76f, 1f);

    public void IncreaseFeature(GameObject obj) {
        int points = 0;
        string feature = obj.name;
        switch(obj.name) {
            case "Design":
                if (design < 10) {
                    design++;
                }
                points = design;
                break;

            case "Marketing":
                if (marketing < 10) {
                    marketing++;
                }
                points = marketing;
                break;

            case "Engineering":
                if (engineering < 10) {
                    engineering++;
                }
                points = engineering;
                break;

            default:
                break;
        }

        Transform items = obj.transform.Find("Points");
        for (int i=0; i < items.childCount; i++) {
            if (i < points)
                items.GetChild(i).GetComponent<UITexture>().color = activeColor;
            else
                items.GetChild(i).GetComponent<UITexture>().color = inactiveColor;
        }

        UpdateProgressRequired(obj, feature, points);
    }

    public void DecreaseFeature(GameObject obj) {
        int points = 0;
        string feature = obj.name;
        switch(feature) {
            case "Design":
                if (design > 0) {
                    design--;
                }
                points = design;
                break;

            case "Marketing":
                if (marketing > 0) {
                    marketing--;
                }
                points = marketing;
                break;

            case "Engineering":
                if (engineering > 0) {
                    engineering--;
                }
                points = engineering;
                break;

            default:
                break;
        }

        Transform items = obj.transform.Find("Points");
        for (int i=0; i < items.childCount; i++) {
            if (i < points)
                items.GetChild(i).GetComponent<UITexture>().color = activeColor;
            else
                items.GetChild(i).GetComponent<UITexture>().color = inactiveColor;
        }

        UpdateProgressRequired(obj, feature, points);
    }

    private void UpdateProgressRequired(GameObject obj, string feature, int points) {
        int estimatedWeeks = product.EstimatedCompletionWeeks(feature, points, gm.playerCompany);
        UILabel label = obj.transform.Find("Progress Required").gameObject.GetComponent<UILabel>();
        label.text = "about " + estimatedWeeks.ToString() + " weeks";
    }

    public void BeginProductDevelopment() {
        int totalWeeks = product.EstimatedCompletionWeeks(gm.playerCompany);
        UIManager.Instance.Confirm("Are you happy with this product configuration? It will take about " + totalWeeks.ToString() + " weeks to develop.", BeginProductDevelopment_, null);
    }

    private void BeginProductDevelopment_() {
        gm.playerCompany.StartNewProduct(productTypes, design, marketing, engineering);
        SendMessageUpwards("Close");
    }
}


