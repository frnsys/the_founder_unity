using UnityEngine;
using System.Collections;

public class UIOfficeManager : Singleton<UIOfficeManager> {
    private GameData data;
    private Company company;

    public Camera UICamera;
    public Camera OfficeCamera;

    // TO DO how this works will need to be updated with office upgrades
    public LockedOfficeArea labs;
    public LockedOfficeArea comms;

    public GameObject officeArea;
    public GameObject officeUIPanel;
    public GameObject upgradeButton;

    public GameObject employeeHUDs;
    public GameObject employeeGroup;
    public GameObject employeePrefab;
    public GameObject employeeHUDPrefab;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        Company.WorkerHired += OnHired;
        Company.WorkerFired += OnFired;
    }

    void OnDisable() {
        Company.WorkerHired -= OnHired;
        Company.WorkerFired -= OnFired;
    }

    public Office currentOffice;
    void OnHired(Worker w, Company c) {
        if (c == company) {
            // A lot of manual set up, there may be a better way.
            GameObject obj = Instantiate(employeePrefab) as GameObject;
            obj.transform.parent = employeeGroup.transform;

            RandomlyPlaceEmployee(obj);
            UIEmployee uie = obj.GetComponent<UIEmployee>();
            uie.worker = w;
            w.avatar = obj;
            if (w.texture != null)
                uie.GetComponent<MeshRenderer>().material.mainTexture = w.texture;

            GameObject hud = NGUITools.AddChild(employeeHUDs, employeeHUDPrefab);

            Transform happiness = hud.transform.Find("Happiness");
            UIFollowTarget uift = happiness.GetComponent<UIFollowTarget>();
            uift.target = uie.HUDtarget;
            uift.gameCamera = OfficeCamera;
            uift.uiCamera = UICamera;
            uie.happinessLabel = happiness.GetComponent<UILabel>();

            Transform hudtext = hud.transform.Find("HUDText");
            uift = hudtext.GetComponent<UIFollowTarget>();
            uift.target = uie.HUDtarget;
            uift.gameCamera = OfficeCamera;
            uift.uiCamera = UICamera;
            uie.hudtext = hudtext.GetComponent<HUDText>();

            uie.HUDgroup = hud;

            // Office upgrade is available!
            if (currentOffice.nextOffice != null &&
                c.workers.Count >= currentOffice.nextOffice.employeesRequired) {
                GameEvent.Trigger(GameEvent.LoadNoticeEvent("Upgrade the office"));
                upgradeButton.SetActive(true);
            }
        }
    }

    void OnFired(Worker w, Company c) {
        if (c == company) {
            Destroy(w.avatar);
        }
    }

    public void UpgradeOffice() {
        Office next = currentOffice.nextOffice;
        UIManager.Instance.Confirm(string.Format("Are you sure want to upgrade your office? It will cost you {0}.", next.cost),
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
        Destroy(currentOffice.gameObject);

        // Set up the new office.
        GameObject obj = Instantiate(next.gameObject) as GameObject;
        currentOffice = obj.GetComponent<Office>();
        obj.transform.parent = officeArea.transform;

        // Reposition workers in the new office.
        foreach (Worker w in company.workers) {
            if (w.avatar != null) {
                RandomlyPlaceEmployee(w.avatar);
            }
        }

        // Load existing perks for the new office.
        foreach (Perk p in company.perks) {
            currentOffice.ShowPerk(p);
        }

        upgradeButton.SetActive(false);
    }

    public void SetupOfficeUI(GameObject[] prefabs, Transform[] targets) {
        // Cleanup existing office UI.
        Transform trans = officeUIPanel.transform;
        while (trans.childCount > 0)
            NGUITools.Destroy(trans.GetChild(0).gameObject);

        for (int i=0; i < prefabs.Length; i++) {
            GameObject obj = NGUITools.AddChild(officeUIPanel, prefabs[i]);
            obj.GetComponent<UIFollowTarget>().target = targets[i];
        }
    }

    void RandomlyPlaceEmployee(GameObject obj) {
        // Random starting location in the office.
        // x_1 + Random.value*(x_2 - x_1)
        Vector4 bounds = currentOffice.bounds;
        float x = bounds[0] + (bounds[2] - bounds[0]) * Random.value;
        float z = bounds[1] + (bounds[3] - bounds[1]) * Random.value;
        obj.transform.localPosition = new Vector3(x, 7f, z);
    }

    public void Load(GameData d) {
        data = d;
        company = d.company;
    }

    public void BuyLabs() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (company.Pay(cost)) {
            data.LabsAccessible = true;
            labs.accessible = true;
        }
    }

    public void BuyComms() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (company.Pay(cost)) {
            data.CommsAccessible = true;
            comms.accessible = true;
        }
    }

}
