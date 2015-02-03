using UnityEngine;
using System;
using System.Collections;

public class OfficeArea : MonoBehaviour {
    // The box which highlights this area as locked.
    public GameObject lockBox;

    // The objects within this area which can be interacted with.
    public UIObject[] interactiveObjects;

    // The UI buttons overlaid these interactive objects.
    public UIButton[] buttons;

    // The UI button which unlocks this area.
    public UIButton unlockButton;

    private bool _accessible = false;
    public bool accessible {
        get { return _accessible; }
        set {
            _accessible = value;

            lockBox.SetActive(!value);
            unlockButton.gameObject.SetActive(!value);
            foreach (UIButton b in buttons) {
                b.gameObject.SetActive(value);
            }
            foreach (UIObject obj in interactiveObjects) {
                obj.enabled = value;
            }
        }
    }
}
