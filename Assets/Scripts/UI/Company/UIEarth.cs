using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIEarth : MonoBehaviour {
    private Location location_;
    public Location location {
        set {
            if (location_ != null) {
                // Set the marker representing the old location.
                SetLocationMarker(location_);
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

    void OnEnable() {
        // Hide all locations which are not yet unlocked.
        IEnumerable<string> locs = GameManager.Instance.unlocked.locations.Select(l => l.name);
        foreach (Transform child in transform) {
            if (!locs.Contains(child.name)) {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void SetLocationMarker(Location loc) {
        Transform mt = transform.Find(loc.name);
        if (mt) {
            GameObject marker = mt.gameObject;
            if (GameManager.Instance.playerCompany.HasLocation(loc)) {
                marker.renderer.material = ownedMaterial;
            } else {
                marker.renderer.material = unownedMaterial;
            }
        }
    }

    public Material ownedMaterial;
    public Material activeMaterial;
    public Material unownedMaterial;
    public float rotationSpeed = 0.1f;

    private IEnumerator RotateToLocation(Quaternion from, Quaternion to) {
        float step = rotationSpeed;
        for (float f = 0f; f <= 1f + step; f += step) {
            transform.rotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

    }
}
