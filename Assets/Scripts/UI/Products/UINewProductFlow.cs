/*
 * New Product Flow
 * ================
 *
 * Selecting a Product Type, Industry, and Market
 * to start development of a new product.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    private enum Stage {
        PRODUCTTYPE,
        INDUSTRY,
        MARKET,
        CONFIRM,
        POINTS
    }
    // Begin on Product Type selection.
    private Stage stage = Stage.PRODUCTTYPE;

    // Lots of UI elements we will be managing...
    public UILabel aspectLabel;
    public UITexture background;
    public UICenterOnChild gridCenter;
    public UIGrid grid;
    public UIScrollView scrollView;
    public UIButton selectButton;
    public UILabel selectLabel;
    public UIWidget completedScreen;
    public UIWidget pointsScreen;

    // The prefabs for all product aspects and features.
    public GameObject productAspectPrefab;
    public GameObject featurePrefab;

    // Keep track of the selected product aspects.
    private ProductType productType;
    private Industry industry;
    private Market market;

    // The available feature points.
    private FeaturePoints featurePoints;

    // These labels are used to display
    // the user's current selections.
    public UILabel productTypeLabel;
    public UILabel industryLabel;
    public UILabel marketLabel;

    // These display the user's selections
    // on the final confirmation screen.
    public UILabel finalProductTypeLabel;
    public UILabel finalIndustryLabel;
    public UILabel finalMarketLabel;

    void OnEnable() {
        gm = GameManager.Instance;
        featurePoints = GameManager.Instance.playerCompany.featurePoints;
        background.color = new Color(0.61f,0.067f,0.57f,1f);

        gridCenter.onFinished = OnCenter;

        LoadProductTypes();
    }

    private void OnCenter() {
        // Re-enable the select button
        // when centering the item after a swipe is complete.
        selectButton.isEnabled = true;
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the select button
            // while dragging.
            selectButton.isEnabled = false;
        }
    }

    public void Select() {
        switch (stage) {
            case Stage.PRODUCTTYPE:
                stage = Stage.INDUSTRY;
                aspectLabel.text = "INDUSTRY";
                background.color = new Color(1f,1f,1f,1f);

                productType = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as ProductType;
                productTypeLabel.text = productType.ToString();
                productTypeLabel.gameObject.SetActive(true);

                LoadIndustries();
                break;

            case Stage.INDUSTRY:
                stage = Stage.MARKET;
                aspectLabel.text = "MARKET";
                background.color = new Color(1f,0.69f,1f,1f);

                industry = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as Industry;
                industryLabel.text = industry.ToString();
                industryLabel.gameObject.SetActive(true);

                LoadMarkets();
                break;

            case Stage.MARKET:
                stage = Stage.CONFIRM;
                background.color = new Color(0.2f,0.69f,0.7f,1f);

                // Hide the aspect label.
                aspectLabel.gameObject.SetActive(false);

                market = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as Market;
                marketLabel.text = market.ToString();
                marketLabel.gameObject.SetActive(true);

                scrollView.gameObject.SetActive(false);
                completedScreen.gameObject.SetActive(true);
                finalProductTypeLabel.text = productType.ToString();
                finalIndustryLabel.text = industry.ToString();
                finalMarketLabel.text = market.ToString();

                int totalPoints = productType.points + industry.points + market.points;
                int diffPoints = totalPoints - GameManager.Instance.playerCompany.availableProductPoints;
                if (diffPoints <= 0) {
                    selectLabel.text = "OK";
                } else {
                    selectButton.isEnabled = false;
                    selectLabel.text = "You need " + diffPoints.ToString() + " more product points.";
                }
                break;

            case Stage.CONFIRM:
                stage = Stage.POINTS;
                UpdateFeaturePoints();

                completedScreen.gameObject.SetActive(false);
                pointsScreen.gameObject.SetActive(true);

                selectLabel.text = "Start";
                break;

            case Stage.POINTS:
                gm.playerCompany.StartNewProduct(productType, industry, market);

                // TEMPORARY, this has bad performance.
                UIRoot.Broadcast("UpdateDevelopingProducts");

                UIRoot.Broadcast("ClosePopup");
                break;
        }
    }


    // ===============================================
    // Loading =======================================
    // ===============================================

    private void LoadProductTypes() {
        ClearGrid();
        foreach (ProductType a in gm.unlocked.productTypes) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void LoadIndustries() {
        ClearGrid();
        foreach (Industry a in gm.unlocked.industries) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void LoadMarkets() {
        ClearGrid();
        foreach (Market a in gm.unlocked.markets) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }


    // ===============================================
    // Point Allocation ==============================
    // ===============================================

    private FeatureSet features = new FeatureSet();

    public void AddPoint(GameObject obj) {
        UpdateFeature(obj, true);
    }
    public void SubtractPoint(GameObject obj) {
        UpdateFeature(obj, false);
    }
    private void UpdateFeature(GameObject obj, bool inc) {
        string featureName = obj.transform.Find("Feature Name").GetComponent<UILabel>().text;

        if (inc) {
            featurePoints = features.Increment(featureName, featurePoints);
        } else {
            featurePoints = features.Decrement(featureName, featurePoints);
        }

        if (features[featureName] < 0)
            features[featureName] = 0;
        obj.transform.Find("Total").GetComponent<UILabel>().text = features[featureName].ToString();
        UpdateFeaturePoints();
    }

    private void UpdateFeaturePoints() {
        pointsScreen.transform.Find("Available Charisma").GetComponent<UILabel>().text = "CHA:" + featurePoints.charisma.ToString();
        pointsScreen.transform.Find("Available Cleverness").GetComponent<UILabel>().text = "CLE:" + featurePoints.cleverness.ToString();
        pointsScreen.transform.Find("Available Creativity").GetComponent<UILabel>().text = "CRE:" + featurePoints.creativity.ToString();
    }


    // ===============================================
    // Utility =======================================
    // ===============================================

    private void ClearGrid() {
        while (grid.transform.childCount > 0)
            NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject);
    }

    // The grid has to be re-wrapped whenever
    // its contents change, since UIWrapContent
    // caches the grid's contents on its start.
    private void WrapGrid() {
        NGUITools.DestroyImmediate(grid.gameObject.GetComponent<UIWrapContent>());
        UIWrapContent wrapper = grid.gameObject.AddComponent<UIWrapContent>();

        // Disable culling since it screws up the grid's layout.
        wrapper.cullContent = false;

        // The wrapper's item width is the same as the grid's cell width, duh
        wrapper.itemSize = (int)grid.cellWidth;
    }
}


