using UnityEngine;
using System.Collections;

public class UIProductType : MonoBehaviour {
    private ProductType productType_;
    public ProductType productType {
        get { return productType_; }
        set {
            productType_ = value;
            label.text = productType_.name;
            productObject.GetComponent<MeshFilter>().mesh = productType_.mesh;
        }
    }

    void Update() {
        UIAnimator.Rotate(productObject);
    }

    public UILabel label;
    public GameObject productObject;
}


