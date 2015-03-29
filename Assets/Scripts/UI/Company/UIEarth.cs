using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIEarth : MonoBehaviour {
    private Location location_;
    public Location location {
        set {
            if (location_ != null) {
                // Update the old location's marker.
                SetLocationMarker(location_);
            }

            location_ = value;

            // Only go to the new marker if the location is unlocked.
            if (GameManager.Instance.unlocked.locations.Contains(location_)) {
                // Locations without an alt mesh are on earth.
                if (location_.altMesh == null) {
                    GetComponent<MeshFilter>().mesh = defaultMesh;
                    renderer.material = defaultMat;
                    ShowMarkers();

                    // Highlight the new marker, if there is one.
                    Transform mt = markers.transform.Find(location_.name);
                    if (mt != null) {
                        GameObject newMarker = mt.gameObject;
                        newMarker.renderer.material = activeMaterial;
                    }


                } else {
                    GetComponent<MeshFilter>().mesh = location_.altMesh;
                    renderer.material = location_.altMat;
                    HideMarkers();
                }

                // Rotate.
                Quaternion from = transform.rotation;
                Quaternion to = Quaternion.Euler(location_.rotation);
                StartCoroutine(RotateToLocation(from, to));
            }

        }
        get {
            return location_;
        }
    }

    void HideMarkers() {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(false);
        }
    }

    void ShowMarkers() {
        foreach (Transform t in transform) {
            t.gameObject.SetActive(true);
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
        Transform mt = markers.transform.Find(loc.name);
        if (mt) {
            GameObject marker = mt.gameObject;
            if (GameManager.Instance.playerCompany.HasLocation(loc)) {
                marker.renderer.material = ownedMaterial;
            } else {
                marker.renderer.material = unownedMaterial;
            }
        }
    }

    public void LockLocation(Location loc) {
        Transform mt = markers.transform.Find(loc.name);
        if (mt)
            mt.gameObject.SetActive(false);
    }

    public Material ownedMaterial;
    public Material activeMaterial;
    public Material unownedMaterial;
    public float rotationSpeed = 0.1f;

    public GameObject markers;

    public Mesh defaultMesh;
    public Material defaultMat;

    private IEnumerator RotateToLocation(Quaternion from, Quaternion to) {
        float step = rotationSpeed;
        for (float f = 0f; f <= 1f + step; f += step) {
            transform.rotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

    }
}
