using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UILabor : MonoBehaviour {
    public UILabel amount;
    public GameObject display;
    public MeshFilter displayMesh;

    public Mesh creativityMesh;
    public Mesh clevernessMesh;
    public Mesh charismaMesh;
    public Mesh breakthroughMesh;

    private Stat stat_;
    public Stat stat {
        get {
            return stat_;
        }
        set {
            stat_ = value;
            switch (stat.name) {
                case "Charisma":
                    displayMesh.mesh = charismaMesh;
                    break;
                case "Creativity":
                    displayMesh.mesh = creativityMesh;
                    break;
                case "Cleverness":
                    displayMesh.mesh = clevernessMesh;
                    break;
                case "Breakthrough":
                    displayMesh.mesh = breakthroughMesh;
                    break;
            }
            amount.text = string.Format("+{0:F0}", stat_.value);
        }
    }

    void OnEnable() {
        StartCoroutine(Scale(0f, 1f));
        Invoke("Disappear", 2);
    }

    private IEnumerator Scale(float from, float to, Action<GameObject> cb = null) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 0.1f;

        for (float f = 0f; f <= 1f + step; f += step) {
            transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        if (cb != null) {
            cb(gameObject);
        }
    }

    void Disappear() {
        StartCoroutine(Scale(1f, 0f, Deactivate));
    }

    void Deactivate(GameObject obj) {
        stat_ = null;
        gameObject.SetActive(false);
    }

    void Update() {
        UIAnimator.Rotate(display);
    }
}
