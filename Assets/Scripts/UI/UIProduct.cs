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
        switch(product.state) {
            case Product.State.DEVELOPMENT:
                progress.text = ((int)(product.progress)).ToString();
                break;

            case Product.State.LAUNCHED:
                progress.text = "$" + ((int)(product.revenueEarned)).ToString();
                break;
        }
    }
}


