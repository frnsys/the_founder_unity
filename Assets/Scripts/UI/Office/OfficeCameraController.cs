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
        get { return bounds[2]; }
    }
    private float rBound {
        get { return bounds[0]; }
    }
    private float tBound {
        get { return bounds[3]; }
    }
    private float bBound {
        get { return bounds[1]; }
    }

    void Start() {
        om = UIOfficeManager.Instance;
        ResetPosition();
    }

    public void ResetPosition() {
        Vector3 position = officeCamera.transform.position;
        Vector4 camBounds = om.currentOffice.cameraBounds;
        Vector2 sizeLimits = om.currentOffice.cameraSizeLimits;
        position.x =  camBounds[0] + (camBounds[2] - camBounds[0])/2;
        position.y =  camBounds[1] + (camBounds[3] - camBounds[1])/2;
        officeCamera.transform.position = position;
        officeCamera.orthographicSize = sizeLimits[0] + (sizeLimits[1] - sizeLimits[0])/2;
        UpdateBounds();
    }

    private Vector4 bounds = new Vector4();
    void UpdateBounds() {
        // The bounds should be set at the office's lower camera orthographic size limit at a 16/9 aspect ratio.
        float delta = officeCamera.orthographicSize - om.currentOffice.cameraSizeLimits[0];
        float aspect = 16f/9f;
        float deltaWidth = (delta * 2f/aspect);
        float deltaHeight = 2*delta;
        Vector4 camBounds = om.currentOffice.cameraBounds;

        // X
        bounds[0] = camBounds[0] + deltaWidth/2f;
        bounds[2] = camBounds[2] - deltaWidth/2f;

        // Y
        bounds[1] = camBounds[1] + deltaHeight/2f;
        bounds[3] = camBounds[3] - deltaHeight/2f;

        // Leave some minimal wiggle room along the Y.
        if (bounds[3] <= bounds[1]) {
            float mid = camBounds[1] + (camBounds[3] - camBounds[1])/2;
            bounds[1] = mid - 1f;
            bounds[3] = mid + 1f;
        }
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
            if (newSize < om.currentOffice.cameraSizeLimits[0]) {
                newSize = om.currentOffice.cameraSizeLimits[0];
            } else if (newSize > om.currentOffice.cameraSizeLimits[1]) {
                newSize = om.currentOffice.cameraSizeLimits[1];
            }
            officeCamera.orthographicSize = newSize;
            UpdateBounds();
        }
    }
}
