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
        get { return om.currentOffice.cameraBounds[2] * BoundAdjustment(); }
    }
    private float rBound {
        get { return om.currentOffice.cameraBounds[0] * BoundAdjustment(); }
    }
    private float tBound {
        get { return om.currentOffice.cameraBounds[3] * BoundAdjustment(); }
    }
    private float bBound {
        get { return om.currentOffice.cameraBounds[1] * BoundAdjustment(); }
    }

    void Start() {
        om = UIOfficeManager.Instance;
        ResetPosition();
    }

    public void ResetPosition() {
        Vector3 position = officeCamera.transform.position;
        Vector4 camBounds = om.currentOffice.cameraBounds;
        position.x =  camBounds[0] + (camBounds[2] - camBounds[0])/2;
        position.y =  camBounds[1] + (camBounds[3] - camBounds[1])/2;
        officeCamera.transform.position = position;
    }

    float BoundAdjustment() {
        // 6 because that is the orthographic size at which the bounds are configured.
        return Mathf.Sqrt(6/officeCamera.orthographicSize);
    }

    void OnDrag(Vector2 delta) {
        if (!Input.touchSupported || Input.touchCount == 1) {
            // Invert y direction so it feels more "natural".
            delta.y *= -1f;

            // Apply speed modification. When the camera is closer (lower orthographic size),
            // it should pan more slowly.
            Vector3 delta3 = delta *= 0.0012f * officeCamera.orthographicSize;
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
            touchDelta *= 0.008f;

            float newSize = officeCamera.orthographicSize += touchDelta;
            if (newSize < 4) {
                newSize = 4;
            } else if (newSize > 12) {
                newSize = 12;
            }
            officeCamera.orthographicSize = newSize;
        }
    }
}
