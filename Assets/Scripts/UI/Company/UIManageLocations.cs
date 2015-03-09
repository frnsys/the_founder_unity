using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageLocations : UIFullScreenPager {
    private GameManager gm;
    private UIEarth earth;

    public GameObject locationPrefab;
    public GameObject earthObject;

    void OnEnable() {
        gm = GameManager.Instance;
        earth = earthObject.GetComponent<UIEarth>();
        LoadLocations();
    }

    private void LoadLocations() {
        gridCenter.onFinished = OnCenter;
        ClearGrid();

        //foreach (Location l in gm.unlocked.locations) {
        foreach (Location l in Location.LoadAll()) { // TEMP
            GameObject locationItem = NGUITools.AddChild(grid.gameObject, locationPrefab);
            locationItem.GetComponent<UILocationItem>().location = l;

            earth.SetLocationMarker(l);
        }
        earth.location = grid.transform.GetChild(0).GetComponent<UILocationItem>().location;

        Adjust();
    }

    private void OnCenter() {
        UILocationItem item = gridCenter.centeredObject.GetComponent<UILocationItem>();
        if (item != null) {
            earth.location = item.location;
        }
    }
}
