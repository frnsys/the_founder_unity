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
    public GameObject officeUIPanel;
    public GameObject upgradeButton;

    public GameObject employeeHUDs;
    public GameObject employeeGroup;
    public GameObject employeePrefab;
    public GameObject employeeHUDPrefab;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(DayNightCycle());
    }

    void OnEnable() {
        Company.WorkerHired += OnHired;
        Company.WorkerFired += OnFired;
    }

    void OnDisable() {
        Company.WorkerHired -= OnHired;
        Company.WorkerFired -= OnFired;
    }

    public void SetupFollowTarget(UIEmployee uie, UIFollowTarget uift) {
        uift.target = uie.HUDtarget;
        uift.gameCamera = OfficeCamera;
        uift.uiCamera = UICamera;
    }

    public Color dayColor;
    public Color nightColor;
    private IEnumerator DayNightCycle() {
        float step = 0.00006f;

        while(true) {
            for (float f = 0f; f <= 1f + step; f += step) {
                OfficeCamera.backgroundColor = Color.Lerp(dayColor, nightColor, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }

            for (float f = 0f; f <= 1f + step; f += step) {
                OfficeCamera.backgroundColor = Color.Lerp(nightColor, dayColor, Mathf.SmoothStep(0f, 1f, f));
                yield return null;
            }
        }
    }

    void OnHired(Worker w, Company c) {
        if (c == company) {
            SpawnWorker(w);

            // Office upgrade is available!
            if (currentOffice.nextOffice != null &&
                !officeUpgradeTriggered &&
                c.workers.Count >= currentOffice.nextOffice.employeesRequired) {
                officeUpgradeTriggered = true;

                // TESTING, the production delay should be longer
                StartCoroutine(GameEvent.DelayTrigger(GameEvent.LoadNoticeEvent("Upgrade the office"), 5f));

                upgradeButton.GetComponent<UIFollowTarget>().target.localPosition = currentOffice.upgradeButtonPosition;
                upgradeButton.SetActive(true);
            }
        }
    }

    public void SpawnWorker(Worker w) {
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
        if (w.texture != null)
            uie.GetComponent<MeshRenderer>().material.mainTexture = w.texture;

        GameObject hud = NGUITools.AddChild(employeeHUDs, employeeHUDPrefab);

        Transform happiness = hud.transform.Find("Happiness");
        UIFollowTarget uift = happiness.GetComponent<UIFollowTarget>();
        SetupFollowTarget(uie, uift);
        uie.happinessLabel = happiness.GetComponent<UILabel>();

        Transform hudtext = hud.transform.Find("HUDText");
        uift = hudtext.GetComponent<UIFollowTarget>();
        SetupFollowTarget(uie, uift);
        uie.hudtext = hudtext.GetComponent<HUDText>();

        uie.HUDgroup = hud;
    }

    void OnFired(Worker w, Company c) {
        if (c == company) {
            Destroy(w.avatar);
        }
    }

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
        foreach (Worker w in company.allWorkers) {
            if (w.avatar != null) {
                w.avatar.SetActive(false);
                RandomlyPlaceEmployee(w.avatar);
                w.avatar.SetActive(true);
            }
        }

        // Load existing perks for the new office.
        foreach (Perk p in company.perks) {
            currentOffice.ShowPerk(p);
        }

        upgradeButton.SetActive(false);
        officeUpgradeTriggered = false;
    }

    public void SetupOfficeUI(GameObject[] prefabs, Transform[] targets) {
        // Cleanup existing office UI.
        Transform trans = officeUIPanel.transform;
        while (trans.childCount > 0)
            NGUITools.Destroy(trans.GetChild(0).gameObject);

        for (int i=0; i < prefabs.Length; i++) {
            GameObject obj = NGUITools.AddChild(officeUIPanel, prefabs[i]);
            UIFollowTarget uift = obj.GetComponent<UIFollowTarget>();
            uift.target = targets[i];
            uift.disableIfInvisible = false;
            uift.gameCamera = OfficeCamera;
        }
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

    // Returns a random unoccupied desk.
    public Office.Desk RandomDesk() {
        List<Office.Desk> unoccupiedDesks = currentOffice.desks.Where(d => !d.occupied).ToList();

        if (unoccupiedDesks.Count == 0)
            return null;

        return unoccupiedDesks[Random.Range(0, unoccupiedDesks.Count -1)];
    }

    public void Load(GameData d) {
        data = d;
        company = d.company;

        // Spawn the cofounder as a worker.
        SpawnWorker(d.company.founders[0]);
    }
}
