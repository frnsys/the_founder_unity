using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProductCompletedAlert: UIEffectAlert {
    public UILabel nameLabel;
    public UILabel aspectsLabel;

    public Product product {
        set {
            nameLabel.text = value.name;
            bodyLabel.text = value.description;
            aspectsLabel.text = value.productType.name + " for " + value.industry.name + " " + value.market.name;
            Extend(bodyLabel.height);

            // NOT USED
            //RenderEffects(product.effects);

            // -1 because by default there is space for about 1 effect.
            //Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }
}


