using UnityEngine;
using System.Collections;

// A confirmation popup with a text input field.
public class UIInputConfirm : UIConfirm {
    public UIInput input;

    protected override void Extend(int amount) {
        amount = (amount/2) + 125;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }
}
