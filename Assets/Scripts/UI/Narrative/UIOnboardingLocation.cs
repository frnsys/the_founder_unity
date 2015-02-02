using UnityEngine;
using System.Collections;

public class UIOnboardingLocation : MonoBehaviour {
    private Location location_;
    public Location location {
        get { return location_; }
        set {
            location_ = value;
            label.text = location_.name;
            description.text = location_.description;
            earth.transform.rotation = Quaternion.Euler(location_.rotation);
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture background;
    public GameObject earth;
}


