using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquippableItem : Item {
    public EquippableItem(string name_, float cost_, List<Industry> industries_,
        List<ProductType> productTypes_, List<Market> markets_
    ) : base(name_, cost_, industries_, productTypes_, markets_) {
    }
}