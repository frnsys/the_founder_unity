/*
 * This class is used to manage a separate vertical and horizontal
 * UIDragScrollView so that it locks to one direction during dragging.
 */

using UnityEngine;
using System.Collections;

public class UIScrollManager : MonoBehaviour {
    public UIDragScrollView horizontalDrag;
    public UIDragScrollView verticalDrag;

    private bool directionLock;

    void OnDrag(Vector2 delta) {
        if (!directionLock) {
            directionLock = true;

            // Horizontal
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
                verticalDrag.enabled = false;
                horizontalDrag.enabled = true;
                horizontalDrag.scrollView.onStoppedMoving += UnlockHorizontal;

            // Vertical
            } else {
                horizontalDrag.enabled = false;
                verticalDrag.enabled = true;
                verticalDrag.scrollView.onStoppedMoving += UnlockVertical;
            }
        }
    }

    void UnlockHorizontal() {
        directionLock = false;
        horizontalDrag.scrollView.onStoppedMoving -= UnlockHorizontal;
    }
    void UnlockVertical() {
        directionLock = false;
        verticalDrag.scrollView.onStoppedMoving -= UnlockVertical;
    }
}
