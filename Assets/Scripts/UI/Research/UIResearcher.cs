using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearcher : MonoBehaviour {
    private Worker worker_;
    public Worker worker {
        get { return worker_; }
        set {
            worker_ = value;
            empty = false;
            displayObject.GetComponent<MeshRenderer>().material = worker_.material;
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

    public GameObject displayObject;
    public GameObject addLabel;
}


