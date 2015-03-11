using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
    public UIGrid grid;
    public UIMenuButton menuButton;

    void OnEnable() {
        grid.Reposition();
    }

    public void Activate(string item) {
        GameObject menuItem = grid.transform.Find(item).gameObject;
        menuItem.SetActive(true);

        grid.Reposition();

        menuButton.Wiggle();
        menuItem.GetComponent<UIMenuItem>().wiggle = true;
    }

    public void Deactivate(string item) {
        grid.transform.Find(item).gameObject.SetActive(false);
        grid.Reposition();
    }
}
