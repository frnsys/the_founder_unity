using UnityEngine;
using System.Collections;

public class HypePuck : MonoBehaviour {
    public static event System.Action Fired;

    public float speed = 4000f;
    public float speedLimit = 4000f;
    public bool fired;
    protected bool seen;
    public bool isVisible {
        get { return !(seen && fired && !renderer.isVisible); }
    }

    void Update() {
        if (renderer.isVisible)
            seen = true;
    }

    public void Reset() {
        fired = false;
        seen = false;
        gameObject.SetActive(true);
        transform.position = new Vector3(0,-0.768f,0);
        rigidbody2D.velocity = Vector3.zero;
    }

    public void Fire(Vector2 dir) {
        if (dir.y < 0)
            dir.y *= -1;
        rigidbody2D.AddForce(Vector2.ClampMagnitude(dir * speed, speedLimit));
        fired = true;

        if (Fired != null)
            Fired();
    }

}
