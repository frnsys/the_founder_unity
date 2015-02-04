using UnityEngine;
using System.Collections;

public class UIOfficeViewManager : Singleton<UIOfficeViewManager> {
    private GameData data;
    private Company company;

    public Camera UICamera;
    public Camera OfficeCamera;

    public OfficeArea labs;
    public OfficeArea comms;
    public OfficeArea market;

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

    void OnHired(Worker w, Company c) {
        if (c == company) {
            // A lot of manual set up, there may be a better way.
            GameObject obj = Instantiate(employeePrefab) as GameObject;
            obj.transform.parent = employeeGroup.transform;

            // TO DO don't hardcode these.
            // Random starting location in the office.
            float x = Random.value * 12 - 7.5f;
            float z = Random.value * 4.5f - 3.5f;
            obj.transform.localPosition = new Vector3(x, 7f, z);
            UIEmployee uie = obj.GetComponent<UIEmployee>();
            uie.worker = w;

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
        }
    }

    void OnFired(Worker w, Company c) {
        if (c == company) {
            Destroy(w.avatar);
        }
    }

    public void Load(GameData d) {
        data = d;
        company = d.company;

        // Setup which office areas are accessible.
        labs.accessible = d.LabsAccessible;
        comms.accessible = d.CommsAccessible;
        market.accessible = d.MarketAccessible;
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

    public void BuyMarket() {
        // TO DO don't hardcode this
        float cost = 20000;
        if (company.Pay(cost)) {
            data.MarketAccessible = true;
            market.accessible = true;
        }
    }

}
