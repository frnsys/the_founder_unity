/*
 * Controls panning and zooming of the office camera.
 */

using UnityEngine;
using System.Collections;

public class OfficeCameraController : MonoBehaviour {
    public Camera officeCamera;
    private UIOfficeManager om;

    // These bounds are based on the bounds of the current office.
    private float lBound {
        get { return om.currentOffice.bounds[3] - 2f; }
    }
    private float rBound {
        get { return om.currentOffice.bounds[0] - 2f; }
    }
    private float tBound {
        get { return om.currentOffice.bounds[2] + 5; }
    }
    private float bBound {
        get { return om.currentOffice.bounds[1] + 5; }
    }

    void Start() {
        om = UIOfficeManager.Instance;
    }

    void OnDrag(Vector2 delta) {
        if (!Input.touchSupported || Input.touchCount == 1) {
            // Invert y direction so it feels more "natural".
            delta.y *= -1f;

            // Apply speed modification. When the camera is closer (lower orthographic size),
            // it should pan more slowly.
            Vector3 delta3 = delta *= 0.005f * officeCamera.orthographicSize;
            Vector3 position = officeCamera.transform.position + delta3;

            // Bounding
            if (position.x > lBound) {
                position.x = lBound;
            } else if (position.x < rBound) {
                position.x = rBound;
            }

            if (position.y > tBound) {
                position.y = tBound;
            } else if (position.y < bBound) {
                position.y = bBound;
            }

            officeCamera.transform.position = position;
        }

        if (Input.touchCount > 1) {
            PinchZoom();
        }
    }

    private float touchDelta = 0.0f;
    private Vector2 prevDist = new Vector2(0,0);
    private Vector2 curDist = new Vector2(0,0);
    void PinchZoom() {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            touchDelta = curDist.magnitude - prevDist.magnitude;

            // Flip directions, since pinch is zooming out and pulling is zooming.
            touchDelta *= -1f;

            // Apply speed modification.
            touchDelta *= 0.005f;

            float newSize = officeCamera.orthographicSize += touchDelta;
            if (newSize < 1) {
                newSize = 1;
            } else if (newSize > 6) {
                newSize = 6;
            }
            officeCamera.orthographicSize = newSize;
        }
    }
}
