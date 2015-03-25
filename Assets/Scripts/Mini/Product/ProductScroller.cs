using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductScroller : MonoBehaviour {
    public GameObject player;

    void OnEnable() {
        // TO DO set box collider to be full screen
    }

    void OnDrag(Vector2 delta) {
        // TO DO add bounding to movement
        // TO DO tilt left/right
        Vector3 drag = Vector3.zero;
        drag.x = delta.x * 0.005f;
        drag.y = delta.y * 0.005f;
        player.transform.localPosition += drag;
    }
}
