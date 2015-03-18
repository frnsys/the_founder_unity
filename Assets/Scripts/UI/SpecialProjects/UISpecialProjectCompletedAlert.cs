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

            Extend(bodyLabel.height);
            RenderEffects(value.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    void Update() {
        UIAnimator.Rotate(projectObject);
    }
}


