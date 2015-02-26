using UnityEngine;
using System.Collections;

public class UIOnboardingVertical : MonoBehaviour {
    private Vertical vertical_;
    public Vertical vertical {
        get { return vertical_; }
        set {
            vertical_ = value;
            label.text = vertical_.name;
            description.text = vertical_.description;

            displayObject.GetComponent<MeshFilter>().mesh = vertical_.mesh;
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture background;
    public GameObject displayObject;

    void Update() {
        // Rotate the product, fancy.
        displayObject.transform.Rotate(0,0,-50*Time.deltaTime);
    }
}


