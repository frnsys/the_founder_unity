using UnityEngine;
using System.Collections;

// Adapted from: http://stackoverflow.com/a/11497674/1097920
public class CameraController : MonoBehaviour {
    Camera camera;
    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    float z = 0.0f;

    // TO DO These are hardcoded for now,
    // but eventually should be generated from the office layout.
    private float lBound = 4f;
    private float rBound = -4f;
    private float tBound = 4f;
    private float bBound = 1.5f;

    void Start() {
        camera = GetComponent<Camera>();
    }

    void Update(){
        // Drag to pan.
        if(Input.GetMouseButtonDown(0)){
            hit_position = Input.mousePosition;
            camera_position = transform.position;

        }
        if(Input.GetMouseButton(0)){
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }

        // Pinch to zoom.
        PinchZoom();
    }

    void LeftMouseDrag(){
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction so that terrain appears to move with the mouse.
        direction.y *= -1f;

        // Multiply it by a scalar to increase the speed of camera movement
        // (really, how much a drag moves the camera).
        direction *= 4f;

        Vector3 position = camera_position + direction;

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

        transform.position = position;
    }

    private float touchDelta = 0.0F;
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
            touchDelta *= 0.05f;

            float newSize = camera.orthographicSize += touchDelta;
            if (newSize < 1) {
                newSize = 1;
            } else if (newSize > 6) {
                newSize = 6;
            }
            camera.orthographicSize = newSize;
        }
    }
}
