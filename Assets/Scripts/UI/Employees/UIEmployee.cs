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
    private NavMeshAgent agent;

    public Vector3 target;

    void Start() {
        StartCoroutine(Working());

        agent = GetComponent<NavMeshAgent>();
        target = RandomLocation();
    }

    void Update() {
        agent.SetDestination(target);
        // Check if we've reached the destination
        if (Vector3.Distance(agent.nextPosition, agent.destination) <= agent.stoppingDistance) {
            target = RandomLocation();
        }
    }

    void OnDestroy() {
        Destroy(HUDgroup);
    }

    // Temporary, to get employees moving about the office.
    Vector3 RandomLocation() {
        // TO DO don't hardcode these values.
        float x = Random.value * 9f - 5f;
        float z = Random.value * 20f + 10f;
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
}
