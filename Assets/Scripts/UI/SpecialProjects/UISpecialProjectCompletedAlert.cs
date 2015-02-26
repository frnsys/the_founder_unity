using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UISpecialProjectCompletedAlert : UIEffectAlert {
    public UILabel nameLabel;
    public GameObject projectObject;

    public SpecialProject specialProject {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;

            projectObject.GetComponent<MeshFilter>().mesh = value.mesh;
            projectObject.GetComponent<MeshRenderer>().material.mainTexture = value.texture;

            Extend(bodyLabel.height);
            RenderEffects(value.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }


    void Update() {
        // Rotate the product, fancy.
        projectObject.transform.Rotate(0,0,0.5f);
    }
}


