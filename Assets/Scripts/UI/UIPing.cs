using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPing : MonoBehaviour {
    public UILabel label;
    public void Set(string note, Color color) {
        label.text = note;
        label.color = color;
    }
}
