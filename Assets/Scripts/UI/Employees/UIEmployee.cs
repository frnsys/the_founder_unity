using UnityEngine;
using System.Collections;

public class UIEmployee : MonoBehaviour {
    public Transform HUDtarget;
    public Color workColor;
    public Color breakthroughColor;
    public Color unhappyColor;
    public Color happyColor;

    [HideInInspector]
    public GameObject HUDgroup;
    [HideInInspector]
    public HUDText hudtext;
    [HideInInspector]
    public UILabel happinessLabel;

    [HideInInspector]
    public Worker worker;

    [HideInInspector]
    public GameObject laborObj;
    public GameObject laborPrefab;

    [HideInInspector]
    public bool atDesk = false;
    [HideInInspector]
    public Office.Desk desk;

    private NavMeshAgent agent;
    private Company company;
    private bool idling;

    [HideInInspector]
    public Vector3 target;

    void Start() {
        company = GameManager.Instance.playerCompany;
        StartCoroutine(Working());

        agent = GetComponent<NavMeshAgent>();
        target = RandomTarget();
    }

    void Update() {
        // Move to target if not at desk and not idling.
        if (!idling && !atDesk) {
            agent.SetDestination(target);

            // Check if we've reached the destination
            // For this to work, the stoppingDistance has to be about 1.
            if (Vector3.Distance(agent.nextPosition, agent.destination) <= agent.stoppingDistance) {

                // If going to a desk...
                if (desk != null) {
                    atDesk = true;

                } else {

                    // May randomly go to desk.
                    if (company.developing && Random.value > 0.05f * worker.productivity.value) {
                        GoToDesk();

                    // Else, continue wandering around.
                    } else {
                        StartCoroutine(Pause());
                        target = RandomTarget();
                    }
                }
            }
        }
    }

    IEnumerator Pause() {
        idling = true;
        yield return new WaitForSeconds(1f + Random.value * 3f);
        idling = false;
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

    public void GoToDesk() {
        Debug.Log("GOING TO DESK!");
        desk = UIOfficeManager.Instance.RandomDesk();
        if (desk != null) {
            target = transform.parent.TransformDirection(desk.transform.position);
            desk.occupied = true;
        }
    }
    public void LeaveDesk() {
        Debug.Log("LEAVING DESK!");
        atDesk = false;
        desk.occupied = false;
        desk = null;
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

            if (company.developing && atDesk && laborObj == null) {
                // Decide whether or not to work
                // or leave the desk.
                // TO DO may need to tweak this value.
                if (Random.value < 0.05f/worker.productivity.value) {
                    LeaveDesk();
                } else {
                    laborObj = NGUITools.AddChild(HUDgroup, laborPrefab);

                    Stat stat;
                    if (Random.value <= 0.02f * worker.happiness.value) {
                        stat = new Stat("Breakthrough", Randomize(
                            (worker.creativity.value + worker.cleverness.value + worker.charisma.value)/3f
                        ));
                    } else {
                        float roll = Random.value;
                        if (roll <= 0.33) {
                            stat = new Stat("Design", Randomize(worker.creativity.value ));
                        } else if (roll <= 0.66) {
                            stat = new Stat("Engineering", Randomize(worker.cleverness.value ));
                        } else {
                            stat = new Stat("Marketing", Randomize(worker.charisma.value ));
                        }
                    }

                    laborObj.GetComponent<UILabor>().stat = stat;

                    UIFollowTarget uift = laborObj.GetComponent<UIFollowTarget>();
                    UIOfficeManager.Instance.SetupFollowTarget(this, uift);
                }

                // Breakthrough!
                // TO DO this should be based on employee happiness
                //if (Random.value < 0.4) {
                    //hudtext.Add("BRK!", breakthroughColor, 0f);
                //} else {
                    //hudtext.Add(worker.productivity.value, workColor, 0f);
                //}
            }

            yield return new WaitForSeconds(2 * Random.value);
        }
    }

    private float Randomize(float value) {
        return Mathf.Max(1, (0.5f + Random.value) * value);
    }

    // Double click to force back to desk,
    // depending on happiness.
    void OnDoubleClick() {
        if (!atDesk && desk == null) {
            if (Random.value <= 0.5f * worker.happiness.value) {
                GoToDesk();
            }
        }
    }
}
