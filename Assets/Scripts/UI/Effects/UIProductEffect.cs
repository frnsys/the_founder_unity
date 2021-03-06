using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProductEffect : UIBuffEffect {
    public void Set(ProductEffect pe) {
        List<string> targets = new List<string>();
        foreach (ProductType x in pe.productTypes) {
            // some leftover product types get caught here
            if (x != null) {
                targets.Add(x.name);
            }
        }
        foreach (Vertical x in pe.verticals) {
            targets.Add(x.name);
        }
        string target = string.Join(", ", targets.ToArray());
        target += " products";
        base.Set(pe.buff, target);
    }
}


