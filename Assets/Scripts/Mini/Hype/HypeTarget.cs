using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HypeTarget : MonoBehaviour {
    public static Color cascadeColor = new Color(0.74f, 1.0f, 0.95f, 0.2f);
    public static Color firstColor = new Color(0.74f, 1.0f, 0.95f, 0.4f);
    public static Color hypeColor = new Color(0.34f, 0.98f, 0.57f);

    public Mesh outrageMesh;
    public Mesh happyMesh;
    public Texture moodTexture;

    public enum Function {
        Linear,
        Sin,
        Cos
    }
    public static Function RandomFunction {
        get {
            Array funcs = Enum.GetValues(typeof(Function));
            return (Function)funcs.GetValue(UnityEngine.Random.Range(0, funcs.Length));
        }
    }

    public enum Type {
        FriendFamily,
        Blogger,
        Journalist,
        ThoughtLeader
    }
    public static Type RandomType {
        get {
            Array types = Enum.GetValues(typeof(Type));
            return (Type)types.GetValue(UnityEngine.Random.Range(0, types.Length));
        }
    }

    public float hypePoints = 1f;
    public float speed = 1f;
    public float distance = 5f;
    public float cascadeRadius = 10f;
    public int numPucks;
    public float bonus = 1f;
    public Function function;
    public GameObject rangeDisplay;
    public GameObject model;
    public GameObject hypeTargetPuckPrefab;
    public HUDText hudtext;

    private Transform gameBoard;

    private Vector2 start;
    new private bool enabled = true;

    public static event System.Action<float> Scored;
    public static int activePucks = 0;
    private static int[] notes = new int[] { 0, 5, 7, -3, -5, -7, -12, -24, -19, -17, -15 };
    private bool hit;

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
        rangeDisplay.transform.localScale = Vector2.zero;
        rangeDisplay.GetComponent<SpriteRenderer>().color = cascadeColor;

        UIFollowTarget uift = hudtext.GetComponent<UIFollowTarget>();
        uift.gameCamera = UIManager.Instance.uiCamera;
        uift.uiCamera = UIManager.Instance.uiCamera;
        hudtext.fontSize = 48;

        StartCoroutine("Move");
    }

    public void Reset() {
        // Re-enable for the new puck.
        enabled = true;
    }

    public void Setup(HypeMinigame hg, Type type) {
        model.renderer.material.mainTexture = hg.blebTextures[UnityEngine.Random.Range(0, hg.blebTextures.Length - 1)];
        function = RandomFunction;
        distance = 0.2f + UnityEngine.Random.value * 1.2f;

        int type_i = (int)type;
        speed = 1 + UnityEngine.Random.value * (type_i + 1);
        numPucks = 1 + type_i * 2;
        hypePoints = (int)(1 + type_i + UnityEngine.Random.value * (type_i + 1));

        gameBoard = hg.gameBoard;

        transform.localPosition = new Vector2(-0.5f + UnityEngine.Random.value, -0.4f + (type_i * 0.4f));
        start = transform.position;
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
                HypeTarget hto = other.GetComponent<HypeTargetPuck>().owner;
                if (hto == this) {
                    return;

                // Otherwise, this puck is used up and becomes inactive.
                } else {
                    // If this is an outrage puck,
                    // there's a chance this turns into an outrage target
                    if (hto.bonus < 0 && UnityEngine.Random.value < 0.4f) {
                        Outrage();
                    // Same for happy pucks
                    } else if (hto.bonus > 1 && UnityEngine.Random.value < 0.25f) {
                        Happy();
                    }

                    activePucks--;
                    Destroy(other.gameObject);
                }
            }

            // Can only be hit once per puck.
            enabled = false;
            Cascade();
        }
    }

    public void Cascade() {
        StartCoroutine(Highlight());

        float points = hit ? hypePoints/10 : hypePoints;
        points *= bonus;

        hit = true;
        hudtext.Add(string.Format("+{0:0.#}", points), hypeColor, 0f);
        if (Scored != null)
            Scored(points);

        // Spawn pucks.
        for (int i=0; i<numPucks; i++) {
            GameObject puckObj = Instantiate(hypeTargetPuckPrefab) as GameObject;
            puckObj.transform.parent = gameBoard;
            puckObj.transform.position = transform.position;
            HypeTargetPuck htp = puckObj.GetComponent<HypeTargetPuck>();
            htp.owner = this;

            // Change color for outrage/happy pucks
            if (bonus < 0) {
                puckObj.GetComponent<SpriteRenderer>().color = Color.red;
            } else if (bonus > 1) {
                puckObj.GetComponent<SpriteRenderer>().color = Color.yellow;
            }

            htp.Fire();

            // Keep track of active pucks so we know when
            // everything is finished.
            activePucks++;
        }
    }

    public void Outrage() {
        model.GetComponent<MeshFilter>().mesh = outrageMesh;
        model.renderer.material.mainTexture = moodTexture;
        model.transform.localScale = new Vector3(2f, 2f, 2f);
        model.transform.localEulerAngles = new Vector3(-90f, -144f, 0);
        bonus = -1f;
    }

    public void Happy() {
        model.GetComponent<MeshFilter>().mesh = happyMesh;
        model.renderer.material.mainTexture = moodTexture;
        model.transform.localScale = new Vector3(2f, 2f, 2f);
        model.transform.localEulerAngles = new Vector3(-82f, -180f, 0);
        bonus = 1.5f;
    }

    // Highlight the target.
    public IEnumerator Highlight() {
        StartCoroutine(ShowHit());

        float note = notes[UnityEngine.Random.Range(0, notes.Length)];
        audio.pitch =  Mathf.Pow(2, note/12.0f);
        audio.Play();

        model.renderer.material.color = new Color(0f,1f,0f);
        yield return new WaitForSeconds(0.25f);
        model.renderer.material.color = new Color(0.4f,0.4f,0.4f);
    }

    // Show the circular pop around the target.
    IEnumerator ShowHit() {
        Vector2 endScale = transform.localScale * cascadeRadius * 6;
        float step = 4f;
        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            rangeDisplay.transform.localScale = Vector2.Lerp(rangeDisplay.transform.localScale, endScale, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        for (float f = 0f; f <= 1f + step * Time.deltaTime; f += step * Time.deltaTime) {
            rangeDisplay.transform.localScale = Vector2.Lerp(endScale, Vector2.zero, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }
    }
}
