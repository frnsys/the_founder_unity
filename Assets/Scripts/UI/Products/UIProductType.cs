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

            productObject.GetComponent<MeshFilter>().mesh = productType_.mesh;
        }
    }

    void Update() {
        // Rotate the product, fancy.
        productObject.transform.Rotate(0,0,-50*Time.deltaTime);
    }

    public UILabel label;
    public UILabel infrastructure;
    public GameObject productObject;
}


