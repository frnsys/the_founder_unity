/*
 * Product Effect
 * ===================
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIProductEffect : UIBuffEffect {
    // TO DO
    // make icon public and replaceable.

    public void Set(ProductEffect pe) {
        List<string> targets = new List<string>();
        foreach (ProductType x in pe.productTypes) {
            targets.Add(x.name);
        }
        foreach (Industry x in pe.industries) {
            targets.Add(x.name);
        }
        foreach (Market x in pe.markets) {
            targets.Add(x.name);
        }
        string target = string.Join(", ", targets.ToArray());
        target += " products";
        base.Set(pe.buff, target);
    }
}


