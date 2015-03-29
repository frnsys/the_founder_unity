using UnityEngine;
using System.Linq;
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

        foreach (Location l in Location.LoadAll().OrderBy(i => i.cost)) {
            GameObject locationItem = NGUITools.AddChild(grid.gameObject, locationPrefab);
            UILocationItem uli = locationItem.GetComponent<UILocationItem>();
            uli.location = l;

            earth.SetLocationMarker(l);

            if (!gm.unlocked.locations.Contains(l)) {
                uli.Lock();
                earth.LockLocation(l);
            }
        }
        earth.location = grid.transform.GetChild(0).GetComponent<UILocationItem>().location;

        Adjust();
    }

    private void OnCenter() {
        UILocationItem item = gridCenter.centeredObject.GetComponent<UILocationItem>();
        if (item != null && !item.locked) {
            earth.location = item.location;
        }
    }
}
