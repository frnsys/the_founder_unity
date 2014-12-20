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

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    public GameObject productTypePrefab;
    public GameObject selectedProductTypePrefab;
    public UIGrid selectedGrid;
    public UISimpleGrid grid;
    public UIButton confirmSelectionButton;

    // Keep track of the selected product aspects.
    private List<ProductType> productTypes;

    // The available feature points.
    private FeaturePoints featurePoints;

    void OnEnable() {
        gm = GameManager.Instance;
        featurePoints = GameManager.Instance.playerCompany.featurePoints;

        LoadProductTypes();

        if (productTypes == null) {
            productTypes = new List<ProductType>();
        }

        if (productTypes.Count == 0) {
            confirmSelectionButton.isEnabled = false;
        } else {
            confirmSelectionButton.isEnabled = true;
        }
    }

    private void LoadProductTypes() {
        grid.Clear();
        foreach (ProductType pt in gm.unlocked.productTypes) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            UIEventListener.Get(productType).onClick += SelectProductType;
            productType.GetComponent<UIProductType>().productType = pt;
        }
        grid.Reposition();
    }

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
            };

            productTypes.Add(pt);
            selectedGrid.Reposition();
        }
    }

    public void ConfirmSelection() {
        Debug.Log("confirming product types");
    }

    // ===============================================
    // Point Allocation ==============================
    // ===============================================

    //private FeatureSet features = new FeatureSet();

    //public void AddPoint(GameObject obj) {
        //UpdateFeature(obj, true);
    //}
    //public void SubtractPoint(GameObject obj) {
        //UpdateFeature(obj, false);
    //}
    //private void UpdateFeature(GameObject obj, bool inc) {
        //string featureName = obj.transform.Find("Feature Name").GetComponent<UILabel>().text;

        //if (inc) {
            //featurePoints = features.Increment(featureName, featurePoints);
        //} else {
            //featurePoints = features.Decrement(featureName, featurePoints);
        //}

        //if (features[featureName] < 0)
            //features[featureName] = 0;
        //obj.transform.Find("Total").GetComponent<UILabel>().text = features[featureName].ToString();
        //UpdateFeaturePoints();
    //}

    //private void UpdateFeaturePoints() {
        //pointsScreen.transform.Find("Available Charisma").GetComponent<UILabel>().text = "CHA:" + featurePoints.charisma.ToString();
        //pointsScreen.transform.Find("Available Cleverness").GetComponent<UILabel>().text = "CLE:" + featurePoints.cleverness.ToString();
        //pointsScreen.transform.Find("Available Creativity").GetComponent<UILabel>().text = "CRE:" + featurePoints.creativity.ToString();
    //}

}


