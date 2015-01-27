using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductAdAlert: UIAlert {
    public UILabel nameLabel;
    public UILabel titleLabel;
    public UILabel aspectsLabel;
    public GameObject productObject;

    public Product product {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;

            productObject.GetComponent<MeshFilter>().mesh = value.mesh;
            productObject.GetComponent<MeshRenderer>().material.mainTexture = value.texture;

            aspectsLabel.text = string.Join(" & ", value.productTypes.Select(pt => pt.name).ToArray());
            Extend(bodyLabel.height);
        }
    }

    public void SetProductAndCompany(Product p, Company c) {
        product = p;

        titleLabel.text = c.name + " has just released:";
    }

    void Update() {
        // Rotate the product, fancy.
        float rotation = productObject.transform.rotation.z;
        productObject.transform.Rotate(0,0,rotation - 1.5f);
    }
}


