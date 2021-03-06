using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIOfficeManager : Singleton<UIOfficeManager> {
    private GameData data;
    private Company company;

    public Camera UICamera;
    public Camera OfficeCamera;
    public Office currentOffice;
    public OfficeCameraController officeCameraController;

    // To prevent the upgrade office notice from triggering multiple times.
    public bool officeUpgradeTriggered;

    public GameObject officeArea;
    public GameObject employeeGroup;
    public GameObject employeePrefab;

    public Material[] employeePathColors;

    public Material RandomColor() {
        return employeePathColors[Random.Range(0, employeePathColors.Length)];
    }

    void Awake() {
        if (officeUpgradeTriggered)
            UIManager.Instance.menu.Show("Upgrade Office");
        else
            UIManager.Instance.menu.Hide("Upgrade Office");
    }

    void OnEnable() {
        Company.WorkerHired += OnHired;
        Company.WorkerFired += OnFired;
    }

    void OnDisable() {
        Company.WorkerHired -= OnHired;
        Company.WorkerFired -= OnFired;
    }

    void OnHired(AWorker w, Company c) {
        if (c == company) {
            SpawnWorker(w);

            // Office upgrade is available!
            if (currentOffice.nextOffice != null &&
                !officeUpgradeTriggered &&
                c.workers.Count >= currentOffice.nextOffice.employeesRequired) {
                officeUpgradeTriggered = true;

                // TESTING, the production delay should be longer
                // TODO this option should always be available
                //GameManager.Instance.eventManager.DelayTrigger(GameEvent.LoadNoticeEvent("Upgrade the office"), 5f);

                // TO DO this needs to be properly loaded based on a save game
                UIManager.Instance.menu.Activate("Upgrade Office");
            }
        }
    }

    public void SpawnWorker(AWorker w) {
        // A lot of manual set up, there may be a better way.
        GameObject obj = Instantiate(employeePrefab) as GameObject;
        obj.transform.parent = employeeGroup.transform;

        // The employee prefab has to start inactive,
        // since we can't use the NavMeshAgent's `SetDestination` method
        // until it is placed on a NavMesh. So we first place the employee,
        // then activate it.
        RandomlyPlaceEmployee(obj);
        obj.SetActive(true);

        UIEmployee uie = obj.GetComponent<UIEmployee>();
        uie.worker = w;
        w.avatar = obj;
        if (w.material != null)
            uie.GetComponent<MeshRenderer>().material = w.material;
    }

    void OnFired(AWorker w, Company c) {
        if (c == company) {
            Destroy(w.avatar);
        }
    }

    static public event System.Action<Office> OfficeUpgraded;
    public void UpgradeOffice() {
        Office next = currentOffice.nextOffice;
        UIManager.Instance.Confirm(string.Format("Are you sure want to upgrade your office? It will cost you {0:C0}.", next.cost),
            delegate() {
                if (company.UpgradeOffice(next)) {
                    UpgradeOffice_(next);
                } else {
                    UIManager.Instance.Alert("We're short on cash for the upgrade.");
                }
            }, null);
    }
    void UpgradeOffice_(Office next) {
        // Destroy the current office.
        //Destroy(currentOffice.gameObject);

        // Disable the current office.
        currentOffice.gameObject.SetActive(false);

        // Set up the new office.
        //GameObject obj = Instantiate(next.gameObject) as GameObject;
        //obj.transform.parent = officeArea.transform;
        GameObject obj = next.gameObject;
        obj.SetActive(true);
        currentOffice = obj.GetComponent<Office>();
        officeCameraController.ResetPosition();

        // Reposition workers in the new office.
        foreach (AWorker w in company.allWorkers) {
            if (w.avatar != null) {
                w.avatar.SetActive(false);
                RandomlyPlaceEmployee(w.avatar);
                w.avatar.SetActive(true);
            }
        }

        // Load existing perks for the new office.
        foreach (APerk p in company.perks) {
            currentOffice.ShowPerk(p);
        }

        UIManager.Instance.menu.Deactivate("Upgrade Office");
        officeUpgradeTriggered = false;

        if (OfficeUpgraded != null)
            OfficeUpgraded(currentOffice);
    }

    void RandomlyPlaceEmployee(GameObject obj) {
        // Random starting location in the office.
        obj.transform.position = RandomLocation();
    }

    public Vector3 RandomLocation() {
        // x_1 + Random.value*(x_2 - x_1)
        Vector4 bounds = currentOffice.bounds;
        float x = bounds[0] + (bounds[2] - bounds[0]) * Random.value;
        float z = bounds[1] + (bounds[3] - bounds[1]) * Random.value;
        return new Vector3(x, 0f, z);
    }

    public Vector3 RandomTarget() {
        Transform target = currentOffice.validEmployeeTargets[Random.Range(0, currentOffice.validEmployeeTargets.Length - 1)];
        return target.position;
    }

    public void Load(GameData d) {
        data = d;
        company = d.company;

        foreach (AWorker worker in d.company.allWorkers) {
            // Spawn the cofounder as a worker.
            SpawnWorker(worker);
        }
    }
}
