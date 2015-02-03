using UnityEngine;
using System.Collections;

public class UIObject : MonoBehaviour {
    public GameObject window;
    public Color colorStart = Color.white;
    public Color colorEnd = Color.green;
    public float duration = 1.0F;
    public bool enabled;

    void OnClick() {
        if (enabled)
            UIManager.Instance.OpenPopup(window);
    }

    void Update() {
        // Cycle the material's colors to draw attention to it.
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        renderer.material.color = Color.Lerp(colorStart, colorEnd, lerp);
    }
}
