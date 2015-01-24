using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEarth : MonoBehaviour {

    private Location location_;
    public Location location {
        set {
            if (location_ != null) {
                // Find the marker representing the old location.
                GameObject oldMarker = transform.Find(location_.name).gameObject;
                oldMarker.renderer.material = inactiveMaterial;
            }

            location_ = value;
            GameObject newMarker = transform.Find(location_.name).gameObject;
            newMarker.renderer.material = activeMaterial;

            Quaternion from = transform.rotation;
            Quaternion to = Quaternion.Euler(location_.rotation);

            StartCoroutine(RotateToLocation(from, to));
        }
        get {
            return location_;
        }
    }

    public Material activeMaterial;
    public Material inactiveMaterial;
    public float rotationSpeed = 0.1f;

    private IEnumerator RotateToLocation(Quaternion from, Quaternion to) {
        float step = rotationSpeed;
        for (float f = 0f; f <= 1f + step; f += step) {
            transform.rotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

    }
}
