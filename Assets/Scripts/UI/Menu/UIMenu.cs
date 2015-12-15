using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
    public UISimpleGrid grid;
    public UIMenuButton menuButton;
    public UIMenuItem newProductButton;
    public Material lockedMat;

    void OnEnable() {
        grid.Reposition();
    }

    public void Activate(string item) {
        UIMenuItem menuItem = GetItem(item);
        menuItem.wiggle = true;
        menuItem.locked = false;
        if (item == "New Product") {
            Show(item);
        } else {
            Transform lockItem = menuItem.transform.Find("Locked");
            if (lockItem != null) {
                menuItem.display.renderer.material = menuItem.mat;
                menuItem.display.GetComponent<SetRenderQueue>().Reset();
                lockItem.gameObject.SetActive(false);
            } else {
                menuItem.gameObject.SetActive(true);
            }
            grid.Reposition();
            menuButton.Wiggle();
        }
    }

    public void Deactivate(string item) {
        if (item == "New Product") {
            Hide(item);
        } else {
            UIMenuItem menuItem = GetItem(item);
            menuItem.locked = true;

            Transform lockItem = menuItem.transform.Find("Locked");
            if (lockItem != null) {
                menuItem.mat = menuItem.display.renderer.material;
                menuItem.display.renderer.material = lockedMat;
                menuItem.display.GetComponent<SetRenderQueue>().Reset();
                lockItem.gameObject.SetActive(true);
            } else {
                menuItem.gameObject.SetActive(false);
            }

            grid.Reposition();
        }
    }

    public void Hide(string item) {
        GetItem(item).gameObject.SetActive(false);
        grid.Reposition();
    }

    public void Show(string item) {
        GetItem(item).gameObject.SetActive(true);
        grid.Reposition();
    }

    private UIMenuItem GetItem(string item) {
        switch (item) {
            case "New Product":
                return newProductButton;
            default:
                return grid.transform.Find(item).gameObject.GetComponent<UIMenuItem>();
        }
    }
}
