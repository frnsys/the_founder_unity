using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPerks : MonoBehaviour {
    private Color activeColor = new Color(1f,1f,1f,1f);
    private Color inactiveColor = new Color(1f,1f,1f,0.75f);

    public GameObject managePerksView;
    public GameObject purchasePerksView;

    public void ShowView(UIButton button) {
        foreach (UIButton b in transform.Find("Subheader").GetComponentsInChildren<UIButton>()) {
            b.defaultColor = inactiveColor;
        }
        button.defaultColor = activeColor;

        switch(button.gameObject.name) {
            case "Purchase":
                purchasePerksView.SetActive(true);
                managePerksView.SetActive(false);
                break;
            case "Manage":
                purchasePerksView.SetActive(false);
                managePerksView.SetActive(true);
                break;
            default:
                break;
        }
    }
}
