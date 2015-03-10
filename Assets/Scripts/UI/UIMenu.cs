using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {
    public UIGrid grid;
    public UIMenuButton menuButton;

    void OnEnable() {
        grid.Reposition();
    }

    public void Activate(string item) {
        grid.transform.Find(item).gameObject.SetActive(true);
        grid.Reposition();
        menuButton.Wiggle();
    }

    public void Deactivate(string item) {
        grid.transform.Find(item).gameObject.SetActive(false);
        grid.Reposition();
    }
}
