using UnityEngine;
using System.Collections;

public class UIMenuButton : MonoBehaviour {
    private IEnumerator wiggler;

    void OnClick() {
        if (wiggler != null) {
            StopCoroutine(wiggler);
            wiggler = null;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void Wiggle() {
        if (wiggler == null) {
            wiggler = UIAnimator.PulseUI(transform, 1f, 1.8f, 4f);
            StartCoroutine(wiggler);
        }
    }
}
