using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductMinigame : MonoBehaviour {

    public Camera camera;
    public GameObject laborPrefab;
    public Mesh[] laborMeshes;
    public GameObject laborGroup;
    public ProductPlayer player;

    public GameObject officeCamera;
    public GameObject officeCameraController;

    private float aggCre = 0;
    private float aggCle = 0;
    private float aggCha = 0;
    private float aggPro = 0;
    private float aggHap = 0;
    private List<AWorker> workers;
    private Product product;

    public void Setup(Product p, Company c) {
        product = p;

        officeCamera.SetActive(false);
        officeCameraController.SetActive(false);
        EventTimer.Pause();

        labors = new List<ProductLabor>();
        workers = c.allWorkers.ToList();
        foreach (AWorker w in workers) {
            aggCre += w.creativity;
            aggCle += w.cleverness;
            aggCha += w.charisma;
            aggPro += w.productivity;
            aggHap += w.happiness;
            GameObject live = NGUITools.AddChild(livesGrid.gameObject, livePrefab);
            live.transform.Find("Object").renderer.material = w.material;
        }
        livesGrid.Reposition();
        player.Setup(workers[0]);
        player.transform.localPosition = Vector3.zero;

        // Create the labor pool.
        int numLabors = (int)(aggPro * 1.5f);
        for (int i=0; i < numLabors; i++) {
            GameObject labor = Instantiate(laborPrefab) as GameObject;
            labor.name = "Labor";
            labor.transform.parent = laborGroup.transform;
            ProductLabor pl = labor.GetComponent<ProductLabor>();
            pl.Reset();
            labors.Add(pl);
        }

        hazardChance = 0.2f + Mathf.Max(-1 * c.opinion.value, 0) / (1000/Mathf.Sqrt(workers.Count));
        powerupChance = 0.0005f + Mathf.Sqrt(aggHap / 1000);

        StartCoroutine(Spawn());
    }

    void Reset() {
        // Clean up labors.
        for (int i = laborGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject go = laborGroup.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        for (int i = livesGrid.transform.childCount - 1; i >= 0; i--) {
            GameObject go = livesGrid.transform.GetChild(i).gameObject;
            Destroy(go);
        }

        creativityPoints = 0;
        charismaPoints = 0;
        clevernessPoints = 0;

        aggCre = 0;
        aggCle = 0;
        aggCha = 0;
        aggPro = 0;
        aggHap = 0;
    }

    private Company company;
    void OnEnable() {
        ProductPlayer.Scored += Scored;
        ProductPlayer.Died += Died;
        ProductPlayer.Hit += Hit;
        company = GameManager.Instance.playerCompany;
    }
    void OnDisable() {
        ProductPlayer.Scored -= Scored;
        ProductPlayer.Died -= Died;
        ProductPlayer.Hit -= Hit;

        officeCamera.SetActive(true);
        officeCameraController.SetActive(true);
        EventTimer.Resume();
        Reset();
    }

    private float hazardChance;
    private float powerupChance;
    private List<ProductLabor> labors;
    IEnumerator Spawn() {
        while (true) {
            ProductLabor labor = labors.FirstOrDefault(l => l.available);
            if (labor != null) {
                labor.available = false;

                if (Random.value < hazardChance) {
                    labor.type = ProductLabor.RandomHazard;
                    labor.name = "Hazard";
                    labor.points = 4f; // so that they are larger scaled
                } else if (Random.value < powerupChance) {
                    labor.type = ProductLabor.RandomPowerup;
                    labor.name = "Powerup";
                    labor.points = 2f; // so that they are larger scaled
                } else {
                    labor.type = ProductLabor.RandomType;
                    labor.name = "Labor";

                    switch (labor.type) {
                        case ProductLabor.Type.Creativity:
                            labor.points = Mathf.Sqrt(1 + Random.value * aggCre);
                            break;
                        case ProductLabor.Type.Cleverness:
                            labor.points = Mathf.Sqrt(1 + Random.value * aggCle);
                            break;
                        case ProductLabor.Type.Charisma:
                            labor.points = Mathf.Sqrt(1 + Random.value * aggCha);
                            break;
                    }
                }

                labor.GetComponent<MeshFilter>().mesh = laborMeshes[(int)labor.type];

                SetupLabor(labor.gameObject);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void SetupLabor(GameObject labor) {
        float x = Random.value;
        float z = camera.WorldToViewportPoint(player.transform.position).z;
        Vector3 start = camera.ViewportToWorldPoint(new Vector3(Random.value, 1, z));
        start.z = 0;
        labor.transform.position = start;
        labor.SetActive(true);

        labor.rigidbody.velocity = Vector3.zero;
        labor.rigidbody.angularVelocity = Vector3.zero;
        labor.rigidbody.isKinematic = true;
        labor.rigidbody.isKinematic = false;

        if (labor.rigidbody.velocity == Vector3.zero) {
            Vector3 dir = new Vector3(0, -1, 0);
            float speed = 200;
            float speedLimit = 200;
            labor.rigidbody.AddForce(Vector3.ClampMagnitude(dir * speed, speedLimit));
        }
    }

    private float creativityPoints;
    private float charismaPoints;
    private float clevernessPoints;
    void Scored(ProductLabor.Type t, float points) {
        switch (t) {
            case ProductLabor.Type.Creativity:
                creativityPoints += points;
                company.developingProduct.design.baseValue = Mathf.Sqrt(creativityPoints/2);
                break;
            case ProductLabor.Type.Charisma:
                charismaPoints += points;
                company.developingProduct.marketing.baseValue = Mathf.Sqrt(charismaPoints/2);
                break;
            case ProductLabor.Type.Cleverness:
                clevernessPoints += points;
                company.developingProduct.engineering.baseValue = Mathf.Sqrt(clevernessPoints/2);
                break;
        }
    }

    void Hit(ProductLabor.Type t, float points) {
        switch (t) {
            case ProductLabor.Type.Outrage:
                charismaPoints -= points;
                if (charismaPoints < 0)
                    charismaPoints = 0;
                break;
            case ProductLabor.Type.Block:
                creativityPoints -= points;
                if (creativityPoints < 0)
                    creativityPoints = 0;
                break;
            case ProductLabor.Type.Bug:
                clevernessPoints -= points;
                if (clevernessPoints < 0)
                    clevernessPoints = 0;
                break;

            case ProductLabor.Type.Coffee:
                StartCoroutine(Coffee());
                break;
            case ProductLabor.Type.Insight:
                StartCoroutine(Insight());
                break;
        }
    }

    IEnumerator Coffee() {
        float oldGravity = player.gravity.gravity;
        player.gravity.gravity *= 2f;
        yield return new WaitForSeconds(6f);
        player.gravity.gravity = oldGravity;
    }

    IEnumerator Insight() {
        player.shield = true;
        yield return new WaitForSeconds(4f);
        player.shield = false;
    }

    void Died() {
        if (workers.Count > 1) {
            workers.RemoveAt(0);
            player.Respawn(workers[0]);
            NGUITools.Destroy(livesGrid.transform.GetChild(0));
            livesGrid.Reposition();
        } else {
            // End the game.
            product.Complete(company);
            gameObject.SetActive(false);
        }
    }

    public UIProgressBar healthBar;
    public UIProgressBar creativityBar;
    public UIProgressBar charismaBar;
    public UIProgressBar clevernessBar;
    public UIGrid livesGrid;
    public GameObject livePrefab;
    void Update() {
        healthBar.value = player.health/player.maxHealth;
        creativityBar.value = creativityPoints/100;
        charismaBar.value = charismaPoints/100;
        clevernessBar.value = clevernessPoints/100;
    }
}
