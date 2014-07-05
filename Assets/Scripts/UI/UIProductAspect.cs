using UnityEngine;
using System.Collections;

public class UIProductAspect : MonoBehaviour {
    public ProductAspect aspect;
    public UILabel label;

    void OnEnable() {
        label.text = aspect.ToString();
    }
}


