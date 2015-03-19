using UnityEngine;
using System.Collections;

public class ProductBlackHole : MonoBehaviour {

    void OnEnable() {
        StartCoroutine("Pulse");
        StartCoroutine(Timeout());
    }
    void OnDisable() {
        StopCoroutine("Pulse");
    }

    void Update() {
        model.Rotate(-100*Time.deltaTime, -50*Time.deltaTime, 0);
    }

    private IEnumerator Timeout() {
        yield return new WaitForSeconds(5 + Random.value * 5);
        transform.parent.gameObject.SetActive(false);
    }

    public Transform model;
    private IEnumerator Pulse() {
        float from = 0.2f;
        float to = 1f;
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 4f;

        while (true) {
            for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
                model.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }

            for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
                model.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.name == "Labor" && !other.rigidbody.isKinematic) {
            other.rigidbody.velocity = Vector3.zero;
            other.rigidbody.angularVelocity = Vector3.zero;
            other.rigidbody.isKinematic = true;
            Destroy(other.gameObject);
        }
    }
}
