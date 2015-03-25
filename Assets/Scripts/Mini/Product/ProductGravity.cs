using UnityEngine;

public class ProductGravity : MonoBehaviour {

    public float gravity = 0.2f;

    public Vector3 Force(Transform other) {
        return (transform.position - other.position).normalized * gravity / (transform.position - other.position).sqrMagnitude;
    }

    void OnTriggerStay(Collider other) {
        if (other.name == "Labor" || other.name == "Hazard") {
            other.rigidbody.AddForce(Force(other.transform));
        }
    }
}
