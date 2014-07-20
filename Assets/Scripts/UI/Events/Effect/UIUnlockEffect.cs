/*
 * Unlock Effect
 * ===================
 *
 */

using UnityEngine;

public class UIUnlockEffect : MonoBehaviour {
    public UILabel label;

    // TO DO
    // make icon public and replaceable.

    public void Set(string name) {
        label.text = name;
    }
}


