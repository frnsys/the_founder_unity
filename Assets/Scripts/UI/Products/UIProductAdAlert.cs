using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductAdAlert: UIAlert {
    public UILabel nameLabel;
    public UILabel companyLabel;
    public GameObject productObject;

    public Product product {
        set {
            nameLabel.text = value.name;
            productObject.GetComponent<MeshFilter>().mesh = value.mesh;
            productObject.GetComponent<MeshRenderer>().material.mainTexture = value.texture;
        }
    }

    public void SetProductAndCompany(Product p, Company c) {
        product = p;
        companyLabel.text = string.Format("By {0}.", c.name);

        string aspects = string.Join("/", p.productTypes.Select(pt => pt.name).ToArray());
        bodyLabel.text = string.Format("We are so excited to introduce our new {0} product today. We believe it embodies our core philosophy: {1}. We love it, and we think you will too.", aspects, c.slogan);
    }

    void Update() {
        // Rotate the product, fancy.
        productObject.transform.Rotate(0,0,0.5f);
    }
}


