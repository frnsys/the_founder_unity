using UnityEngine;

public class ProductGravity : MonoBehaviour {

    public float gravity = 0.2f;
    public Transform player;

    public Vector3 Force(Transform other) {
        return (transform.position - other.position).normalized * gravity / (transform.position - other.position).sqrMagnitude;
    }

    void OnTriggerStay(Collider other) {
        if (other.name == "Labor" || other.name == "Hazard") {
            Vector3 diff = player.position - other.transform.position;
            if (diff.y > 0)
                diff.y *= 20;
            other.rigidbody.AddForce(diff.normalized * gravity);
            //other.rigidbody.AddForce(Force(other.transform));
        }
    }
}
