/*
 * A confirmation (yes/no) popup.
 */

using UnityEngine;
using System.Collections;

public class UIConfirm : UIAlert {
    public void Yes() {
        // temp, this should return true.
        Close_();
    }

    public void No() {
        // temp, this should return false.
        Close_();
    }
}
