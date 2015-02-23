using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HypeTarget : MonoBehaviour {
    public static Color cascadeColor = new Color(0.74f, 1.0f, 0.95f, 0.2f);
    public static Color firstColor = new Color(0.74f, 1.0f, 0.95f, 0.4f);

    public enum Function {
        Linear,
        Sin,
        Cos
    }

    public float hypePoints = 1f;
    public float opinionPoints = 1f;
    public float speed = 1f;
    public float distance = 5f;
    public float cascadeRadius = 10f;
    public int numPucks;
    public Function function;
    public GameObject rangeDisplay;
    public GameObject model;
    public GameObject hypeTargetPuckPrefab;
    public HUDText hudtext;

    private Vector2 start;
    new private bool enabled = true;

    public static event System.Action<float, float> Scored;
    public static event System.Action Completed;
    public static int activePucks = 0;
    private static int[] notes = new int[] { 0, 5, 7, -3, -5, -7, -12, -24, -19, -17, -15 };

    float Y(float x) {
        switch (function) {
            case Function.Linear:
                return start.y;
            case Function.Sin:
                return start.y + Mathf.Sin(x * 0.1f) * 0.5f;
            case Function.Cos:
                return start.y + Mathf.Cos(x * 0.1f) * 0.5f;
        }
        return start.y;
    }

    void Start() {
        start = transform.position;
        rangeDisplay.transform.localScale = Vector2.zero;
        rangeDisplay.GetComponent<SpriteRenderer>().color = cascadeColor;

        model.renderer.material.mainTexture = HypeMinigame.Instance.blebTextures[Random.Range(0, HypeMinigame.Instance.blebTextures.Length - 1)];

        StartCoroutine("Move");
    }

    IEnumerator Move() {
        while (true) {
            float i = Mathf.PingPong(Time.time * speed * 0.1f, distance);
            float x = start.x + i;
            float y = Y(x);
            Vector2 end = new Vector2(x, y);
            transform.position = end;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (enabled) {
            // Check if we are working with the player puck.
            HypePuck puck = other.GetComponent<HypePuck>();

            // If it is the player puck...
            if (puck.GetType() != typeof(HypeTargetPuck)) {
                // Highlight the player puck's circular pop specially.
                rangeDisplay.GetComponent<SpriteRenderer>().color = firstColor;
                other.gameObject.SetActive(false);

            // Otherwise, it is a target puck.
            } else {
                // Ignore the puck if it belongs to this target.
                if (other.GetComponent<HypeTargetPuck>().owner == this) {
                    return;

                // Otherwise, this puck is used up and becomes inactive.
                } else {
                    activePucks--;
                    Destroy(other.gameObject);
                    if (activePucks == 0 && Completed != null) {
                        Completed();
                    }
                }
            }

            // should targets be able to be re-triggered? much nicer cascade effect, but there needs to be _some_ limit.
            //enabled = false;
            Cascade();
        }
    }

    public void Cascade() {
        StartCoroutine(Highlight());
        StartCoroutine(ShowPoints());

        if (Scored != null)
            Scored(hypePoints, opinionPoints);

        // Spawn pucks.
        for (int i=0; i<numPucks; i++) {
            GameObject puckObj = Instantiate(hypeTargetPuckPrefab) as GameObject;
            puckObj.transform.parent = HypeMinigame.Instance.gameBoard;
            puckObj.transform.position = transform.position;
            HypeTargetPuck htp = puckObj.GetComponent<HypeTargetPuck>();
            htp.owner = this;
            htp.Fire();

            // Keep track of active pucks so we know when
            // everything is finished.
            activePucks++;
        }
    }

    IEnumerator ShowPoints() {
        hudtext.Add(hypePoints, new Color(1f,0.9f,0.53f), 0f);
        yield return new WaitForSeconds(2f);
        hudtext.Add(opinionPoints, new Color(0.07f,0.62f,0.95f), 0f);
    }

    // Highlight the target.
    public IEnumerator Highlight() {
        StartCoroutine(ShowHit());

        float note = notes[Random.Range(0, notes.Length)];
        audio.pitch =  Mathf.Pow(2, note/12.0f);
        audio.Play();

        model.renderer.material.color = new Color(0f,1f,0f);
        yield return new WaitForSeconds(0.25f);
        model.renderer.material.color = new Color(0.4f,0.4f,0.4f);
    }

    // Show the circular pop around the target.
    IEnumerator ShowHit() {
        Vector2 endScale = transform.localScale * cascadeRadius * 2;
        float step = 0.04f;
        for (float f = 0f; f <= 1f + step; f += step) {
            rangeDisplay.transform.localScale = Vector2.Lerp(rangeDisplay.transform.localScale, endScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        for (float f = 0f; f <= 1f + step; f += step) {
            rangeDisplay.transform.localScale = Vector2.Lerp(endScale, Vector2.zero, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
