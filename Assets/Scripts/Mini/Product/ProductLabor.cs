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
        StartCoroutine(UIAnimator.Bloop(transform, 0f, 0.1f * points));
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
}
