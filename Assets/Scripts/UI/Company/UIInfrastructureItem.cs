using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfrastructureItem : MonoBehaviour {
    public Color usedColor = new Color(0.34f, 0.98f, 0.57f);
    public Mesh[] meshes;

    public Infrastructure.Type type_;
    public Infrastructure.Type type {
        get { return type_; }
        set {
            type_ = value;
            displayObject.GetComponent<MeshFilter>().mesh = meshes[(int)type_];
            empty = false;
        }
    }
    private bool empty_;
    public bool empty {
        get { return empty_; }
        set {
            empty_ = value;
            displayObject.SetActive(!value);
            addLabel.SetActive(value);
        }
    }
    private bool used_;
    public bool used {
        get { return used_; }
        set {
            used_ = value;
            if (used_)
                GetComponent<UITexture>().color = usedColor;
        }
    }

    public GameObject displayObject;
    public GameObject addLabel;

    void Update() {
        UIAnimator.Rotate(displayObject);
    }
}


