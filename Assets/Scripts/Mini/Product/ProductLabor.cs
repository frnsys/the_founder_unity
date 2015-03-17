using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductLabor : MonoBehaviour {

    public float speed = 100;
    public float speedLimit = 100;

    protected bool seen;
    protected bool isVisible {
        get { return !(seen && !renderer.isVisible); }
    }

    public enum Type {
        Creativity,
        Cleverness,
        Charisma,
        Breakthrough
    }
    public Type type;
    public float points = 1;

    public static Type RandomType {
        get {
            Array types = Enum.GetValues(typeof(Type));
            // types.Length - 1 because w don't want to include Breakthrough.
            return (Type)types.GetValue(UnityEngine.Random.Range(0, types.Length - 1));
        }
    }

    public void Fire() {
        rigidbody.isKinematic = false;
        Vector3 dir = new Vector3(UnityEngine.Random.value - 0.5f, 1, 0);
        rigidbody.AddForce(Vector3.ClampMagnitude(dir * speed, speedLimit));

        // Hacky, but reparent the Labor so that it doesn't move with the worker group.
        transform.parent = transform.parent.parent.parent;
    }

    void OnEnable() {
        StartCoroutine(Scale(0f, 0.1f * points));
    }

    void Update() {
        if (renderer.isVisible)
            seen = true;

        // Check if it is no longer visible,
        // and if it is not kinematic.
        // If it is kinematic, it means
        // it's attached to the product.
        if (!isVisible && !rigidbody.isKinematic) {
            Destroy(gameObject);
        }
    }

    private IEnumerator Scale(float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        Vector3 toScale_ = toScale * 1.2f;
        float step = 0.1f;

        for (float f = 0f; f <= 1f + step; f += step) {
            transform.localScale = Vector3.Lerp(fromScale, toScale_, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        step = 4f;
        for (float f = 0f; f <= 1f + step; f += step) {
            transform.localScale = Vector3.Lerp(toScale_, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
