using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManageCompany : UIFullScreenPager {
    private GameManager gm;

    public GameObject locationPrefab;
    public GameObject verticalPrefab;
    public UIButton locationButton;
    public UIButton verticalButton;
    private bool locationsActive = false;

    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    void OnEnable() {
        gm = GameManager.Instance;

        ToggleManageView();
    }

    public void ToggleManageView() {
        ClearGrid();

        if (locationsActive) {
            LoadVerticals();
            verticalButton.defaultColor = activeColor;
            locationButton.defaultColor = inactiveColor;
        } else {
            LoadLocations();
            locationButton.defaultColor = activeColor;
            verticalButton.defaultColor = inactiveColor;
        }

        locationsActive = !locationsActive;

        Adjust();
    }

    private void LoadLocations() {
        foreach (Location l in gm.unlocked.locations) {
            GameObject locationItem = NGUITools.AddChild(grid.gameObject, locationPrefab);
            locationItem.GetComponent<UILocationItem>().location = l;
        }
    }

    private void LoadVerticals() {
        foreach (Vertical l in gm.unlocked.verticals) {
            GameObject verticalItem = NGUITools.AddChild(grid.gameObject, verticalPrefab);
            verticalItem.GetComponent<UIVerticalItem>().vertical = l;
        }
    }

}
