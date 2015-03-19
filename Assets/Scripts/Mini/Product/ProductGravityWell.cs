using UnityEngine;

public class ProductGravityWell : MonoBehaviour {

    public float gravity = 0.2f;

    public Vector3 Force(Transform other) {
        return (transform.position - other.position).normalized * gravity / (transform.position - other.position).sqrMagnitude;
    }

    void OnTriggerStay(Collider other) {
        if (other.name == "Labor") {
            Vector3 diff = transform.position - other.transform.position;
            other.rigidbody.AddForce(diff.normalized * 10);
        }
    }
}
