using UnityEngine;

public class HypePuck : MonoBehaviour {
    public static event System.Action Completed;

    public float speed = 4000f;
    protected bool fired;
    protected bool seen;
    protected bool isVisible {
        get { return !(seen && fired && !renderer.isVisible); }
    }

    void Update() {
        if (renderer.isVisible)
            seen = true;

        if (Input.GetMouseButton(0)) {
            rigidbody2D.AddForce(new Vector2(0 * speed * Time.deltaTime, 1f * speed * Time.deltaTime));
            fired = true;
        }

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
        transform.position = new Vector3(0,0,0);
        rigidbody2D.velocity = Vector3.zero;
    }

    protected void DoCompleted() {
        if (Completed != null)
            Completed();
    }
}
