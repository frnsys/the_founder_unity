using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIProductDiscoveredAlert : UIEffectAlert {
    public UILabel nameLabel;
    public UILabel aspectsLabel;
    public GameObject[] productObjects;

    public Product product {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;

            for (int i=0; i<value.meshes.Length; i++) {
                productObjects[i].GetComponent<MeshFilter>().mesh = value.meshes[i];
            }

            aspectsLabel.text = string.Join(" + ", value.productTypes.Select(pt => pt.name).ToArray());
            Extend(bodyLabel.height);

            RenderEffects(value.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    public ProductRecipe recipe {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;

            if (value.productTypes.Count() >= 2) {
                productObjects[0].GetComponent<MeshFilter>().mesh = value.productTypes[0].mesh;
                productObjects[1].GetComponent<MeshFilter>().mesh = value.productTypes[1].mesh;
                aspectsLabel.text = string.Join(" + ", value.productTypes.Select(pt => pt.name).ToArray());
            } else {
                aspectsLabel.text = "";
            }
            Extend(bodyLabel.height);

            RenderEffects(value.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    void Update() {
        // Rotate the product, fancy.
        for (int i=0; i<productObjects.Length; i++) {
            UIAnimator.Rotate(productObjects[i]);
        }
    }
}


