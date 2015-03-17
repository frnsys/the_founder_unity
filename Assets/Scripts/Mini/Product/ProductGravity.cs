using UnityEngine;

public class ProductGravity : MonoBehaviour {

    public float gravity = 0.2f;

    void OnTriggerStay(Collider other) {
        if (other.name == "Labor") {
            other.rigidbody.AddForce((transform.position - other.transform.position).normalized * gravity / (transform.position - other.transform.position).sqrMagnitude);
        }
    }
}
