/*
 * New Product Flow
 * ================
 *
 * Select one or more Product Types
 * to start development of a new product.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINewProductFlow : UIFullScreenPager {
    private GameManager gm;

    private enum Stage {
        PRODUCTTYPE,
        POINTS
    }
    // Begin on Product Type selection.
    private Stage stage = Stage.PRODUCTTYPE;

    // Lots of UI elements we will be managing...
    public UILabel aspectLabel;
    public UITexture background;
    public UIButton selectButton;
    public UILabel selectLabel;
    public UIWidget completedScreen;
    public UIWidget pointsScreen;

    public GameObject productAspectPrefab;

    // Keep track of the selected product aspects.
    private List<ProductType> productTypes;

    // The available feature points.
    private FeaturePoints featurePoints;

    // These labels are used to display
    // the user's current selections.
    public UILabel productTypeLabel;

    // These display the user's selections
    // on the final confirmation screen.
    public UILabel finalProductTypeLabel;

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
                stage = Stage.POINTS;
                UpdateFeaturePoints();

                completedScreen.gameObject.SetActive(false);
                pointsScreen.gameObject.SetActive(true);

                selectLabel.text = "Start";
                break;

            case Stage.POINTS:
                gm.playerCompany.StartNewProduct(productTypes);

                UIManager.Instance.ClosePopup();
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
        Adjust();
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

}


