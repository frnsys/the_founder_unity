using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductTarget : MonoBehaviour {

    void Update() {
        transform.Rotate(-50*Time.deltaTime, 0, 0);
    }

    static public event System.Action<ProductLabor.Type, float> Scored;

    public Transform laborGroup;
    void OnTriggerEnter(Collider other) {
        // "Capture" the labor value.
        if (other.name == "Labor" && !other.rigidbody.isKinematic) {
            other.transform.parent = laborGroup;
            other.rigidbody.velocity = Vector3.zero;
            other.rigidbody.angularVelocity = Vector3.zero;
            other.rigidbody.isKinematic = true;

            ProductLabor pl = other.GetComponent<ProductLabor>();

            if (pl.type != ProductLabor.Type.Breakthrough)
                Scored(pl.type, pl.points);
        }
    }

    public void Reset() {
        for (int i = laborGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject go = laborGroup.transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }
}
