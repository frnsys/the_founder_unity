using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageCompany : UIFullScreenPager {
    private GameManager gm;

    public GameObject locationPrefab;
    public GameObject verticalPrefab;
    public GameObject locationVerticalView;

    public GameObject earthObject;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    void OnEnable() {
        gm = GameManager.Instance;

        // First view.
        LoadLocations();
    }

    public void ShowView(UIButton button) {
        ClearGrid();

        foreach (UIButton b in transform.Find("Subheader").GetComponentsInChildren<UIButton>()) {
            b.defaultColor = inactiveColor;
        }
        button.defaultColor = activeColor;

        switch(button.gameObject.name) {
            case "Locations Button":
                locationVerticalView.SetActive(true);
                LoadLocations();
                break;
            case "Verticals Button":
                locationVerticalView.SetActive(true);
                LoadVerticals();
                break;
            default:
                locationVerticalView.SetActive(false);
                break;
        }

        Adjust();
    }

    private void LoadLocations() {
        earthObject.SetActive(true);
        gridCenter.onFinished = OnCenter;

        foreach (Location l in gm.unlocked.locations) {
            GameObject locationItem = NGUITools.AddChild(grid.gameObject, locationPrefab);
            locationItem.GetComponent<UILocationItem>().location = l;
        }
        earthObject.GetComponent<UIEarth>().location = grid.transform.GetChild(0).GetComponent<UILocationItem>().location;
    }

    private void LoadVerticals() {
        earthObject.SetActive(false);
        gridCenter.onFinished = null;

        foreach (Vertical l in gm.unlocked.verticals) {
            GameObject verticalItem = NGUITools.AddChild(grid.gameObject, verticalPrefab);
            verticalItem.GetComponent<UIVerticalItem>().vertical = l;
        }
    }

    private void OnCenter() {
        UILocationItem item = gridCenter.centeredObject.GetComponent<UILocationItem>();
        if (item != null) {
            earthObject.GetComponent<UIEarth>().location = item.location;
        }
    }

}
