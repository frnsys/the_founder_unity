/*
 * A confirmation (yes/no) popup.
 */

using UnityEngine;
using System.Collections;

public class UIConfirm : UIAlert {
    public void Yes() {
        Close();
    }

    public void No() {
        Close();
    }
}
