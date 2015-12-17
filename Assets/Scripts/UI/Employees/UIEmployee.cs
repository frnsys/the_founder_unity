using UnityEngine;
using System.Collections;

public class UIEmployee : MonoBehaviour {
    public Transform HUDtarget;

    [HideInInspector]
    public GameObject HUDgroup;

    [HideInInspector]
    public AWorker worker;

    [HideInInspector]
    public UILabor laborObj;

    [SerializeField, HideInInspector]
    private State state = State.Wandering;

    private NavMeshAgent agent;
    private Company company;

    [HideInInspector]
    public Vector3 target;

    void Start() {
        company = GameManager.Instance.playerCompany;
        agent = GetComponent<NavMeshAgent>();
        target = RandomTarget();
    }

    [System.Serializable]
    private enum State {
        Wandering,
        Idling
    }

    void Update() {
        if (!GameTimer.paused) {
            agent.Resume();
            // Move to target if not idling.
            if (state == State.Wandering) {
                agent.SetDestination(target);

                // Check if we've reached the destination
                // For this to work, the stoppingDistance has to be about 1.
                if (Vector3.Distance(agent.nextPosition, agent.destination) <= agent.stoppingDistance) {
                    // Else, continue wandering around.
                    StartCoroutine(Pause());
                    target = RandomTarget();
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
        // On enable, reset the target.
        target = RandomTarget();
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
}
