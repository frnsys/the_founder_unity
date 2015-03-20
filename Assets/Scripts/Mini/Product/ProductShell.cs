using UnityEngine;
using System.Collections;

public class ProductShell : MonoBehaviour {
    public ProductLabor.Type type_;
    public ProductLabor.Type type {
        get { return type_; }
        set {
            type_ = value;

            switch (type) {
                // Creativity is harder to break.
                case ProductLabor.Type.Creativity:
                    maxHealth = 40;
                    mesh = shellMeshes[0];
                    break;

                case ProductLabor.Type.Charisma:
                    maxHealth = 20;
                    mesh = shellMeshes[1];
                    break;

                // Cleverness is weaker.
                case ProductLabor.Type.Cleverness:
                    maxHealth = 10;
                    mesh = shellMeshes[2];
                    break;
            }
            health = maxHealth;
            meshFilter.mesh = mesh;

            maxTime = 10f;
            time = maxTime;
        }
    }
    public float health;
    public float maxHealth;
    public MeshFilter meshFilter;
    public Mesh hitMesh;
    public Mesh[] shellMeshes;
    private Mesh mesh;

    public float maxTime;
    public float time;

    static public event System.Action Bug;
    static public event System.Action<ProductLabor.Type> Broken;

    void Update() {
        transform.Rotate(-50*Time.deltaTime, 0, 0);

        // Charisma regens health.
        if (type == ProductLabor.Type.Charisma && health < maxHealth) {
            health += 0.01f;

        // Cleverness can cause bugs.
        } else if (type == ProductLabor.Type.Cleverness && Random.value < 0.003f) {
            if (Bug != null)
                Bug();
        }

        time -= 1f * Time.deltaTime;
        if (time <= 0) {
            AudioManager.Instance.PlayProductShellFX("Fizzle");
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.name == "Labor" && !other.rigidbody.isKinematic) {
            ProductLabor pl = other.GetComponent<ProductLabor>();
            if (pl.type == type) {
                health -= pl.points;
                StartCoroutine(Pulse(1.6f, 1.8f));
                AudioManager.Instance.PlayProductShellFX("Hit");
            } else if (pl.type == ProductLabor.Type.Breakthrough) {
                health -= 6;
                StartCoroutine(Pulse(1.6f, 1.8f));
            }
            Destroy(other.gameObject);
        }
    }

    void OnEnable() {
        StartCoroutine(UIAnimator.Scale(transform, 0f, 1.6f, 12f));
        AudioManager.Instance.PlayProductShellFX("Reveal");
        mesh = meshFilter.mesh;
    }

    private IEnumerator Pulse(float from, float to) {
        Vector3 fromScale = new Vector3(from,from,from);
        Vector3 toScale = new Vector3(to,to,to);
        float step = 12f;
        meshFilter.mesh = hitMesh;

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            transform.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            transform.localScale = Vector3.Lerp(toScale, fromScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        meshFilter.mesh = mesh;

        // Destroyed.
        if (health <= 0) {
            AudioManager.Instance.PlayProductShellFX("Destroy");
            gameObject.SetActive(false);
            if (Broken != null)
                Broken(type_);
        }
    }
}
