using UnityEngine;
using System.Collections;

public class ProductPlayer : MonoBehaviour {
    static public event System.Action<ProductLabor.Type, float> Scored;
    static public event System.Action<ProductLabor.Type, float> Hit;
    static public event System.Action Died;

    public ProductGravity gravity;
    private bool _shield = false;
    public bool shield {
        get { return _shield; }
        set {
            _shield = value;
            shieldObj.SetActive(value);
        }
    }
    public GameObject shieldObj;

    void OnTriggerEnter(Collider other) {
        // "Capture" the labor value.
        if (other.name == "Labor") {
            ProductLabor pl = other.GetComponent<ProductLabor>();
            StartCoroutine(Pickup());
            AudioManager.Instance.PlayLaborHitFX();
            if (Scored != null)
                Scored(pl.type, pl.points);
            pl.Reset();

        } else if (other.name == "Hazard") {
            ProductLabor pl = other.GetComponent<ProductLabor>();
            if (!shield) {
                StartCoroutine(Flash());
                health *= 0.5f;
                AudioManager.Instance.PlayPlayerHitFX();
                Hit(pl.type, pl.points);
            }
            pl.Reset();

        } else if (other.name == "Powerup") {
            ProductLabor pl = other.GetComponent<ProductLabor>();
            AudioManager.Instance.PlayPowerupHitFX();
            Hit(pl.type, pl.points);
            pl.Reset();
        }
    }

    public Material hitMat;
    IEnumerator Flash() {
        Material oldMat = renderer.material;
        renderer.material = hitMat;
        StartCoroutine(UIAnimator.Scale(transform, 1f, 1.4f, 20f));
        yield return new WaitForSeconds(0.1f);
        renderer.material = oldMat;
        StartCoroutine(UIAnimator.Scale(transform, 1.4f, 1f, 20f));
    }
    IEnumerator Pickup() {
        StartCoroutine(UIAnimator.Scale(transform, 1f, 1.2f, 20f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UIAnimator.Scale(transform, 1.2f, 1f, 20f));
    }

    public float health;
    public float maxHealth;
    public void Respawn(AWorker w) {
        StartCoroutine(UIAnimator.Scale(transform, 1f, 0, 4f,
            delegate() {
                Setup(w);
            }
        ));
    }

    public void Setup(AWorker w) {
        shield = false;
        renderer.material = w.material;
        maxHealth = w.productivity.value;
        health = maxHealth;
        StartCoroutine(UIAnimator.Bloop(transform, 0, 1f, 4f));
        dead = false;
    }

    void Update() {
        health -= 0.4f * Time.deltaTime;
        if (health <= 0 && !dead && Died != null) {
            AudioManager.Instance.PlayPlayerDeadFX();
            dead = true;
            Died();
        }
    }
    private bool dead = false;

    void OnDrag(Vector2 delta) {
        // TO DO add bounding to movement
        // TO DO tilt left/right
        Vector3 drag = Vector3.zero;
        drag.x = delta.x * 0.005f;
        drag.y = delta.y * 0.005f;
        transform.localPosition += drag;
    }
}
