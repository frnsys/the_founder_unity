using UnityEngine;

public class UIUnlockEffect : MonoBehaviour {
    public UILabel label;

    public void Set(string name) {
        label.text = "Unlocks " + name;
    }
}


