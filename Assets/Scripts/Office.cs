using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Office : MonoBehaviour {
    public Location location;
    public Company company;
    public int size;
    public float baseRent;
    public float rent {
        get {
            if (location != null) {
                return baseRent * location.expensiveness;
            }
            return baseRent;
        }
    }

    void Start() {
    }

    void Update() {
    }
}


