using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductScroller : MonoBehaviour {
    public GameObject workerGroup;

    void OnDrag(Vector2 delta) {
        Vector3 drag = Vector3.zero;
        drag.x = delta.x * 0.005f;
        workerGroup.transform.localPosition = workerGroup.transform.localPosition + drag;
    }
}
