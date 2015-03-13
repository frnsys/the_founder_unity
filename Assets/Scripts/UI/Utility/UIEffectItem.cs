using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIEffectItem : UIEffectAlert {
    void OnEnable() {
        height = gameObject.GetComponent<UIWidget>().height;
    }

    protected override void Extend(int amount) {
        gameObject.GetComponent<UIWidget>().height = height + amount;
    }
}


