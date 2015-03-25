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
    public bool available = true;

    public enum Type {
        Creativity,
        Cleverness,
        Charisma,
        Outrage,
        Block,
        Bug,
        Coffee,
        Insight
    }
    public Type type;
    public float points = 1;

    public static Type RandomType {
        get {
            Array types = Enum.GetValues(typeof(Type));
            return (Type)types.GetValue(UnityEngine.Random.Range(0, 3));
        }
    }

    public static Type RandomHazard {
        get {
            Array types = Enum.GetValues(typeof(Type));
            return (Type)types.GetValue(UnityEngine.Random.Range(3, 6));
        }
    }

    public static Type RandomPowerup {
        get {
            Array types = Enum.GetValues(typeof(Type));
            return (Type)types.GetValue(UnityEngine.Random.Range(6, 8));
        }
    }

    void OnEnable() {
        StartCoroutine(UIAnimator.Bloop(transform, 0f, 0.1f * points, 4f));
    }

    void Update() {
        if (renderer.isVisible)
            seen = true;

        // Check if it is no longer visible.
        if (!isVisible) {
            Reset();
        }
    }

    public void Reset() {
        gameObject.SetActive(false);
        available = true;
        seen = false;
    }
}
