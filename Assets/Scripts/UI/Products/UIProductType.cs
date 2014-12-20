using UnityEngine;
using System.Collections;

public class UIProductType : MonoBehaviour {
    private ProductType productType_;
    public ProductType productType {
        get { return productType_; }
        set {
            productType_ = value;
            label.text = productType_.name;
            infrastructure.text =productType_.requiredInfrastructure.ToString();
            // TO DO this should be required infrastructure
            //points.text = productType_.points.ToString() + "PP";
        }
    }

    public UILabel label;
    public UILabel infrastructure;
}


