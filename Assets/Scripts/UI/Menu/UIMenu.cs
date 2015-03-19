using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
    public UISimpleGrid grid;
    public UIMenuButton menuButton;
    public UIMenuItem[] hudButtons;

    void OnEnable() {
        grid.Reposition();
    }

    public void Activate(string item) {
        UIMenuItem menuItem = GetItem(item);
        menuItem.wiggle = true;
        menuItem.gameObject.SetActive(true);
        grid.Reposition();

        if (item != "New Product" && item != "Research" && item != "Communications")
            menuButton.Wiggle();
    }

    public void Deactivate(string item) {
        GetItem(item).gameObject.SetActive(false);
        grid.Reposition();
    }

    private UIMenuItem GetItem(string item) {
        switch (item) {
            case "New Product":
                return hudButtons[0];
            case "Research":
                return hudButtons[1];
            case "Communications":
                return hudButtons[2];
            default:
                return grid.transform.Find(item).gameObject.GetComponent<UIMenuItem>();
        }
    }
}
