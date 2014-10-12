/*
 * This class is used to manage a separate vertical and horizontal
 * UIDragScrollView so that it locks to one direction during dragging.
 * If you want, you can restrict the scrolling to a single direction by setting
 * the `direction` property.
 */

using UnityEngine;
using System.Collections;

public class UIScrollManager : MonoBehaviour {
    public UIDragScrollView horizontalDrag;
    public UIDragScrollView verticalDrag;

    public enum Movement {
        Unrestricted,
        Horizontal,
        Vertical
    }
    public Movement direction = Movement.Unrestricted;

    private bool directionLock;

    void OnEnable() {
        if (direction == Movement.Horizontal) {
            horizontalDrag.enabled = true;
            verticalDrag.enabled = false;
        } else if (direction == Movement.Vertical) {
            horizontalDrag.enabled = false;
            verticalDrag.enabled = true;
        }
    }

    void OnDrag(Vector2 delta) {
        if (!directionLock && verticalDrag && direction == Movement.Unrestricted) {
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
