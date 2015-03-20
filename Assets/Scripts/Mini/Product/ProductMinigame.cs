using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductMinigame : MonoBehaviour {

    public GameObject[] laborPrefabs;
    public GameObject laborGroup;
    public GameObject workerGroup;
    public ProductShell shell;
    public ProductTarget target;
    public ProductLabor.Type primaryType;
    public GameObject blackHole;

    public GameObject officeCamera;
    public GameObject officeCameraController;

    private float shellChance;

    public void Setup(Product p) {
        officeCamera.SetActive(false);
        officeCameraController.SetActive(false);

        switch (p.Recipe.primaryFeature) {
            case ProductRecipe.Feature.Design:
                primaryType = ProductLabor.Type.Creativity;
                break;
            case ProductRecipe.Feature.Engineering:
                primaryType = ProductLabor.Type.Cleverness;
                break;
            case ProductRecipe.Feature.Marketing:
                primaryType = ProductLabor.Type.Charisma;
                break;
        }

        float workerWidth = 0.5f;
        workers = new List<ProductWorker>();
        List<Worker> ws = GameManager.Instance.playerCompany.productWorkers.ToList();
        for (int i=0; i < ws.Count; i++) {
            GameObject worker = Instantiate(workerPrefab) as GameObject;
            ProductWorker pw = worker.GetComponent<ProductWorker>();
            pw.Setup(ws[i], workerGroup);
            worker.transform.parent = workerGroup.transform;

            Vector3 pos = Vector3.zero;
            pos.x = i * workerWidth - ((workerWidth * ws.Count)/2);
            pos.y = 0.3f;
            worker.transform.localPosition = pos;

            workers.Add(pw);
        }

        // Probability of a shell appearing is based on product difficulty.
        shellChance = 0.0001f * p.Recipe.featureIdeal;

        EventTimer.Pause();
    }

    void OnDisable() {
        officeCamera.SetActive(true);
        officeCameraController.SetActive(true);
        EventTimer.Resume();
        Reset();
    }

    void Reset() {
        // Clean up workers.
        for (int i = workerGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject go = workerGroup.transform.GetChild(i).gameObject;
            Destroy(go);
        }

        // Clean up labors.
        for (int i = laborGroup.transform.childCount - 1; i >= 0; i--) {
            GameObject go = laborGroup.transform.GetChild(i).gameObject;
            Destroy(go);
        }

        creativityPoints = 0;
        charismaPoints = 0;
        clevernessPoints = 0;

        target.Reset();
    }

    private Company company;
    void Start() {
        // This is for testing.
        //StartCoroutine(Spawn());

        ProductShell.Bug += Debugging;
        ProductShell.Broken += Broken;
        ProductTarget.Scored += Scored;
        Product.Completed += Completed;
        company = GameManager.Instance.playerCompany;
    }

    private List<ProductWorker> workers;
    public GameObject workerPrefab;

    void Debugging() {
        List<ProductWorker> freeWorkers = workers.Where(w => w.debugging == 0).ToList();

        if (freeWorkers.Count > 0) {
            freeWorkers[Random.Range(0, freeWorkers.Count)].StartDebugging();
        }
    }

    IEnumerator Spawn() {
        while (true) {
            GameObject labor = Instantiate(laborPrefabs[Random.Range(0, laborPrefabs.Length)]) as GameObject;
            labor.name = "Labor";
            labor.transform.parent = laborGroup.transform;
            labor.transform.localPosition = Vector3.zero;
            labor.SetActive(true);
            labor.GetComponent<ProductLabor>().Fire();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Completed(Product p, Company c) {
        if (c == company) {
            // End the game.
            gameObject.SetActive(false);
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

    // Breaking shells scores you points for that shell's type.
    void Broken(ProductLabor.Type t) {
        Scored(t, 50);
    }


    public UIProgressBar shellHealth;
    public UIProgressBar shellTimer;
    public UIProgressBar productProgressBar;
    public UIProgressBar creativityBar;
    public UIProgressBar charismaBar;
    public UIProgressBar clevernessBar;
    void Update() {
        productProgressBar.value = company.developingProduct.progress;
        creativityBar.value = creativityPoints/200;
        charismaBar.value = charismaPoints/200;
        clevernessBar.value = clevernessPoints/200;

        // Randomly spawn shells.
        if (!shell.active && Random.value < shellChance) {
            if (Random.value < 0.5) {
                shell.type = primaryType;
            } else {
                shell.type = ProductLabor.RandomType;
            }
            shell.gameObject.SetActive(true);
            shellHealth.gameObject.SetActive(true);
            shellTimer.gameObject.SetActive(true);

        } else if (!blackHole.active && Random.value < 0.00001f) {
            // Black hole
            blackHole.SetActive(true);
            blackHole.transform.localPosition = new Vector3(-1.8f + Random.value * 3.6f, 2.25f + Random.value * 2.25f, 0);
        }

        if (shell.active) {
            shellHealth.value = shell.health/shell.maxHealth;
            shellTimer.value = shell.time/shell.maxTime;
        } else {
            shellHealth.gameObject.SetActive(false);
            shellTimer.gameObject.SetActive(false);
        }
    }

}
