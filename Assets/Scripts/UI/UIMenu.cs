using UnityEngine;

public class UIMenu : MonoBehaviour {
    public UIGrid grid;

    void OnEnable() {
        grid.Reposition();
    }

    public void Activate(string item) {
        grid.transform.Find(item).gameObject.SetActive(true);
        grid.Reposition();
    }

    public void Deactivate(string item) {
        grid.transform.Find(item).gameObject.SetActive(false);
        grid.Reposition();
    }
}
