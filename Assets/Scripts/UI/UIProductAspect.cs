using UnityEngine;
using System.Collections;

public class UIProductAspect : MonoBehaviour {
    private ProductAspect aspect_;
    public ProductAspect aspect {
        get { return aspect_; }
        set {
            aspect_ = value;
            label.text = aspect_.ToString();
            description.text = aspect_.description;
        }
    }

    public UILabel label;
    public UILabel description;
}


