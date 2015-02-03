using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageLocations : UIFullScreenPager {
    private GameManager gm;

    public GameObject locationPrefab;
    public GameObject earthObject;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadLocations();
    }

    private void LoadLocations() {
        gridCenter.onFinished = OnCenter;
        ClearGrid();

        UIEarth earth = earthObject.GetComponent<UIEarth>();

        foreach (Location l in gm.unlocked.locations) {
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
            earthObject.GetComponent<UIEarth>().location = item.location;
        }
    }
}
