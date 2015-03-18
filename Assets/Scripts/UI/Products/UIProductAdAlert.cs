using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductAdAlert: UIAlert {
    public UILabel nameLabel;
    public UILabel companyLabel;
    public GameObject[] productObjects;

    public Product product {
        set {
            nameLabel.text = value.name;
            for (int i=0; i<value.meshes.Length; i++) {
                productObjects[i].GetComponent<MeshFilter>().mesh = value.meshes[i];
            }
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
        for (int i=0; i<productObjects.Length; i++) {
            UIAnimator.Rotate(productObjects[i]);
        }
    }
}


