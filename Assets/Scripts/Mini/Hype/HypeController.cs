using UnityEngine;

public class HypeController : MonoBehaviour {
    public HypePuck puck;

    void OnDrag(Vector2 delta) {
        if (!puck.fired)
            puck.Fire(delta);
    }
}
