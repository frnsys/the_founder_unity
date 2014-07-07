using UnityEngine;
using System.Collections;

public class UIProduct: MonoBehaviour {
    private Product product_;
    public Product product {
        get { return product_; }
        set {
            product_ = value;
            label.text = product_.name;
        }
    }

    public UILabel label;
    public UILabel progress;

    void Update() {
        progress.text = product.progress.ToString();
    }
}


