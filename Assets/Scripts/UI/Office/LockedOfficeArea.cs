using UnityEngine;
using System;
using System.Collections;

public class LockedOfficeArea : MonoBehaviour {
    // The box which highlights this area as locked.
    public GameObject lockBox;

    // The group of objects within this area.
    public GameObject objects;

    // The UI buttons overlaid these interactive objects.
    public UIButton[] buttons;

    // The object which unlocks this area.
    public GameObject unlocker;

    private bool _accessible = false;
    public bool accessible {
        get { return _accessible; }
        set {
            _accessible = value;

            lockBox.SetActive(!value);
            unlocker.SetActive(!value);
            objects.SetActive(value);
            foreach (UIButton b in buttons) {
                b.gameObject.SetActive(value);
            }
        }
    }
}
