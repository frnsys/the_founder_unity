using UnityEngine;

public class HypePuck : MonoBehaviour {
    public static event System.Action Completed;

    public float speed = 4000f;
    public float speedLimit = 4000f;
    public bool fired;
    protected bool seen;
    protected bool isVisible {
        get { return !(seen && fired && !renderer.isVisible); }
    }

    void Update() {
        if (renderer.isVisible)
            seen = true;

        // If the puck has gone off screen and nothing was hit,
        // the round is over.
        if (!isVisible && HypeTarget.activePucks == 0)
            DoCompleted();

        if (rigidbody2D.IsSleeping() && fired && HypeTarget.activePucks == 0)
            DoCompleted();
    }

    public void Reset() {
        fired = false;
        seen = false;
        gameObject.SetActive(true);
        transform.position = new Vector3(0,-0.768f,0);
        rigidbody2D.velocity = Vector3.zero;
    }

    public void Fire(Vector2 dir) {
        rigidbody2D.AddForce(Vector2.ClampMagnitude(dir * speed, speedLimit));
        fired = true;
    }

    protected void DoCompleted() {
        if (Completed != null)
            Completed();
    }
}
