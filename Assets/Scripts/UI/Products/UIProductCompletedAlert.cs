using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductCompletedAlert : UIEffectAlert {
    public UILabel nameLabel;
    public UILabel aspectsLabel;
    public GameObject productObject;

    public Product product {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;

            productObject.GetComponent<MeshFilter>().mesh = value.mesh;

            aspectsLabel.text = string.Join(" & ", value.productTypes.Select(pt => pt.name).ToArray());
            Extend(bodyLabel.height);

            RenderEffects(value.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }


    void Update() {
        // Rotate the product, fancy.
        productObject.transform.Rotate(0,0,0.5f);
    }
}


