using UnityEngine;

public class ProductSticky : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.name == "Labor" && !other.rigidbody.isKinematic) {
            other.rigidbody.velocity = Vector3.zero;
            other.rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
