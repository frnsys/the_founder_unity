using UnityEngine;
using System.Collections;

public class UIEmployee : MonoBehaviour {
    public Transform HUDtarget;
    public Color unhappyColor;
    public Color happyColor;

    [HideInInspector]
    public GameObject HUDgroup;
    [HideInInspector]
    public UILabel happinessLabel;

    [HideInInspector]
    public Worker worker;

    [SerializeField, HideInInspector]
    private Office.Desk desk;
    [SerializeField, HideInInspector]
    private State state = State.Wandering;

    private NavMeshAgent agent;
    private LineRenderer line;
    private Company company;

    [HideInInspector]
    public Vector3 target;

    void Start() {
        company = GameManager.Instance.playerCompany;
        StartCoroutine(Working());

        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        target = RandomTarget();
        desk = UIOfficeManager.Instance.RandomDesk();
        desk.occupied = true;

        line.material = UIOfficeManager.Instance.RandomColor();
    }

    [System.Serializable]
    private enum State {
        Wandering,
        Idling,
        GoingToDesk,
        AtDesk
    }

    void Update() {
        if (!GameTimer.paused) {
            agent.Resume();
            // Move to target if not at desk and not idling.
            if (state == State.GoingToDesk || state == State.Wandering) {
                line.SetPosition(0, transform.position);
                agent.SetDestination(target);

                // May randomly go to desk.
                if (state == State.Wandering && company.developing && Random.value < 0.05f * worker.productivity.value) {
                    GoToDesk();
                } else if (state == State.GoingToDesk) {
                    DrawPath(agent.path);
                    line.enabled = true;
                }

                // Check if we've reached the destination
                // For this to work, the stoppingDistance has to be about 1.
                if (Vector3.Distance(agent.nextPosition, agent.destination) <= agent.stoppingDistance) {

                    // If going to a desk...
                    if (state == State.GoingToDesk) {
                        state = State.AtDesk;
                        line.enabled = false;

                    } else {
                        // Else, continue wandering around.
                        StartCoroutine(Pause());
                        target = RandomTarget();
                    }
                }
            }

        // Force-stop the agent for pausing.
        } else {
            agent.Stop(true);
        }
    }

    IEnumerator Pause() {
        state = State.Idling;
        yield return StartCoroutine(GameTimer.Wait(1f + Random.value * 3f));
        state = State.Wandering;
    }

    void OnEnable() {
        // On enable, reset the target and desk.
        target = RandomTarget();
        desk = UIOfficeManager.Instance.RandomDesk();
        desk.occupied = true;
    }

    void OnDisable() {
        // Relinquish the deks.
        desk.occupied = false;
        desk = null;
    }

    void OnDestroy() {
        Destroy(HUDgroup);
    }

    // Temporary, to get employees moving about the office.
    Vector3 RandomLocation() {
        return transform.parent.TransformDirection(UIOfficeManager.Instance.RandomLocation());
    }

    Vector3 RandomTarget() {
        return transform.parent.TransformDirection(UIOfficeManager.Instance.RandomTarget());
    }

    public void GoToDesk() {
        state = State.GoingToDesk;
        target = transform.parent.TransformDirection(desk.transform.position);
    }
    public void LeaveDesk() {
        state = State.Wandering;
        target = RandomTarget();
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

            yield return StartCoroutine(GameTimer.Wait(1.4f * Random.value));
        }
    }

    private float Randomize(float value) {
        return Mathf.Max(1, (0.5f + Random.value) * value);
    }

    // Draw path to employee's target.
    private void DrawPath(NavMeshPath path) {
        // If the path has 1 or no corners, there is no need.
        if (path.corners.Length < 2)
            return;

        // Set the array of positions to the amount of corners.
        line.SetVertexCount(path.corners.Length);

        // Go through each corner and set that to the line renderer's position.
        for(int i=0; i<path.corners.Length; i++){
            line.SetPosition(i, path.corners[i]);
        }
    }
}
