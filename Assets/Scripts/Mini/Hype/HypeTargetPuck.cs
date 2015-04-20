using UnityEngine;

public class HypeTargetPuck : HypePuck {
    // Keep track of the owners so that the pucks
    // don't trigger their owner.
    [HideInInspector]
    public HypeTarget owner;

    void Update() {
        if (renderer.isVisible)
            seen = true;

        // Check if the puck has gone off screen.
        if (!isVisible) {
            HypeTarget.activePucks--;
            Destroy(gameObject);
        }
    }

    public void Fire() {
        // Fire the puck in a random downwards direction.
        float x = (-1 + Random.value * 2) * speed * Time.deltaTime;
        float y = -1 * speed * Time.deltaTime;

        // There is a slight chance of it firing backwards.
        if (Random.value < 0.05) {
            y = 1 * speed * Time.deltaTime;
        }
        rigidbody2D.AddForce(new Vector2(x, y));
        fired = true;
    }

}
