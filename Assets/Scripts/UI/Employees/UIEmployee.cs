using UnityEngine;
using System.Collections;

public class UIEmployee : MonoBehaviour {
    public GameObject HUDgroup;
    public Transform HUDtarget;
    public HUDText hudtext;
    public UILabel happinessLabel;
    public Color workColor;
    public Color breakthroughColor;
    public Color unhappyColor;
    public Color happyColor;
    public Worker worker;

    void Start() {
        StartCoroutine(MoveTo(RandomLocation()));

        StartCoroutine(Working());
    }

    void OnDestroy() {
        Destroy(HUDgroup);
    }

    // Temporary, to get employees moving about the office.
    Vector3 RandomLocation() {
        // TO DO don't hardcode these values.
        float x = Random.value * 12 - 7.5f;
        float z = Random.value * 4.5f - 3.5f;
        return new Vector3(x, 7f, z);
    }

    IEnumerator Working() {
        while(true) {
            float happy = worker.happiness.value;
            if (happy >= 20) {
                happinessLabel.text = ":D";
                happinessLabel.color = happyColor;
            } else if (happy >= 14) {
                happinessLabel.text = ":)";
                happinessLabel.color = happyColor;
            } else if (happy >= 8) {
                happinessLabel.text = ":\\";
                happinessLabel.color = happyColor;
            } else if (happy >= 4) {
                happinessLabel.text = ":(";
                happinessLabel.color = unhappyColor;
            } else {
                happinessLabel.text = ">:(";
                happinessLabel.color = unhappyColor;
            }

            // Breakthrough!
            if (Random.value < 0.4) {
                hudtext.Add("BRK!", breakthroughColor, 0f);
            } else {
                hudtext.Add(worker.productivity.value, workColor, 0f);
            }

            yield return new WaitForSeconds(2 * Random.value);
        }
    }

    IEnumerator MoveTo(Vector3 to) {
        Vector3 from = transform.localPosition;

        float step = 0.003f;

        for (float f = 0f; f <= 1f + step; f += step) {
            // SmoothStep gives us a bit of easing.
            transform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, f));
            yield return null;
        }

        // Move back and forth, for testing purposes.
        yield return StartCoroutine(MoveTo(RandomLocation()));
    }
}
