using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductWorker : MonoBehaviour {
    public float maxStamina;
    public float stamina;
    public bool fatigued;
    private float staminaRate = 1f;
    public GameObject sweatdrop;

    public float debugging;
    public float maxDebugging;
    private float debugRate = 1f;
    public GameObject bug;

    public float laborProgress;
    public GameObject[] laborPrefabs;
    private List<ProductLabor> labors;
    private ProductLabor.Type laborType;

    private Worker worker;

    void Start() {
        labors = new List<ProductLabor>();
        laborType = ProductLabor.RandomType;
    }

    private GameObject workerGroup;
    public void Setup(Worker w, GameObject wg) {
        workerGroup = wg;
        maxStamina = Mathf.Sqrt(w.productivity.value)/2;
        debugRate = w.cleverness.value/100;
        stamina = maxStamina;
        worker = w;
        renderer.material = w.material;

        if (w.robot)
            staminaRate = 0;
        else
            staminaRate = 0.1f;
    }

    public void StartDebugging() {
        maxDebugging = Random.value * 10;
        debugging = maxDebugging;
        bug.SetActive(true);
    }

    IEnumerator Recover() {
        yield return new WaitForSeconds(4f);
        fatigued = false;
        StopCoroutine("Fatigued");
        stamina = 0.1f;
    }

    void Update() {
        if (!fatigued) {
            if (stamina > 0) {
                if (debugging > 0) {
                    debugging -= debugRate * Time.deltaTime;
                    bug.transform.localScale = new Vector3(debugging/maxDebugging, debugging/maxDebugging, debugging/maxDebugging);
                    if (debugging <= 0)
                        bug.SetActive(false);

                } else if ((labors.Count < 1 && laborType == ProductLabor.Type.Breakthrough) || (labors.Count < 4 && laborType != ProductLabor.Type.Breakthrough)) {
                    if (laborProgress < 1) {
                        stamina -= staminaRate * Time.deltaTime;
                        laborProgress += worker.productivity.value/1.5f * Time.deltaTime;
                    } else {
                        laborProgress = 0;
                        CreatePoint();
                    }
                } else if (stamina < maxStamina) {
                    stamina += staminaRate * Time.deltaTime;
                }

            } else {
                fatigued = true;
                StartCoroutine(Recover());
                StartCoroutine("Fatigued");
            }
        }

        float sd = 1 - stamina/maxStamina;
        sweatdrop.transform.localScale = new Vector3(sd, sd, sd);
    }

    IEnumerator Fatigued() {
        return UIAnimator.Pulse(sweatdrop.transform, 1f, 1.2f, 6f);
    }

    void OnClick() {
        // Release points.
        foreach (ProductLabor l in labors) {
            l.Fire();
        }
        labors.Clear();

        if (Random.value < worker.happiness.value/100) {
            laborType = ProductLabor.Type.Breakthrough;
        } else {
            laborType = ProductLabor.RandomType;
        }
    }

    void OnDrag(Vector2 delta) {
        Vector3 drag = Vector3.zero;
        drag.x = delta.x * 0.005f;
        workerGroup.transform.localPosition = workerGroup.transform.localPosition + drag;
    }

    void CreatePoint() {
        // Spawn points above the employee.
        GameObject labor = Instantiate(laborPrefabs[(int)laborType]) as GameObject;
        labor.name = "Labor";
        labor.transform.parent = transform;
        AudioManager.Instance.PlayLaborFX();

        Vector3 pos = Vector3.zero;
        pos.y = 0.6f + labors.Count * 0.2f;
        labor.transform.localPosition = pos;
        labor.rigidbody.isKinematic = true;

        ProductLabor pl = labor.GetComponent<ProductLabor>();
        pl.type = laborType;

        switch (laborType) {
            case ProductLabor.Type.Creativity:
                pl.points = Random.value * worker.creativity.value;
                break;

            case ProductLabor.Type.Charisma:
                pl.points = Random.value * worker.charisma.value;
                break;

            case ProductLabor.Type.Cleverness:
                pl.points = Random.value * worker.cleverness.value;
                break;
        }

        pl.points = Mathf.Max(Mathf.Sqrt(pl.points), 0.5f);
        labors.Add(pl);
        labor.SetActive(true);
    }
}
