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

        foreach (Worker w in GameManager.Instance.playerCompany.allWorkers) {
            GameObject worker = Instantiate(workerPrefab) as GameObject;
            worker.GetComponent<ProductWorker>().Setup(w);
            worker.transform.parent = workerGroup.transform;
            worker.transform.localPosition = Vector3.zero;
        }

        shellChance = 0.001f * p.Recipe.featureIdeal;
    }

    void OnDisable() {
        officeCamera.SetActive(true);
        officeCameraController.SetActive(true);
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
    }

    private Company company;
    void Start() {
        StartCoroutine(Spawn());
        ProductShell.Bug += Debugging;
        ProductTarget.Scored += Scored;
        Product.Completed += Completed;
        workers = new List<ProductWorker>();
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
            // Convert points into DEM points.
            company.AddPointsToDevelopingProduct("Design", Mathf.Sqrt(creativityPoints/2));
            company.AddPointsToDevelopingProduct("Marketing", Mathf.Sqrt(charismaPoints/2));
            company.AddPointsToDevelopingProduct("Engineering", Mathf.Sqrt(clevernessPoints/2));

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
                break;
            case ProductLabor.Type.Charisma:
                charismaPoints += points;
                break;
            case ProductLabor.Type.Cleverness:
                clevernessPoints += points;
                break;
        }
    }


    public UIProgressBar shellHealth;
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
        }

        if (shell.active) {
            shellHealth.value = shell.health/shell.maxHealth;
        } else {
            shellHealth.gameObject.SetActive(false);
        }
    }

}
