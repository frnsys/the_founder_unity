using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductAdAlert: UIAlert {
    public UILabel nameLabel;
    public UILabel titleLabel;
    public UILabel aspectsLabel;

    public Product product {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;
            aspectsLabel.text = string.Join(" & ", value.productTypes.Select(pt => pt.name).ToArray());
            Extend(bodyLabel.height);
        }
    }

    public void SetProductAndCompany(Product p, Company c) {
        product = p;

        titleLabel.text = c.name + " has just released:";
    }
}


