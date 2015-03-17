using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductMinigame : MonoBehaviour {

    public GameObject[] laborPrefabs;
    public GameObject laborGroup;
    public ProductShell shell;
    public ProductLabor.Type primaryType;

    public void Setup(Product p) {
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
    }

    void Start() {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        while (true) {
            GameObject labor = Instantiate(laborPrefabs[Random.Range(0, laborPrefabs.Length)]) as GameObject;
            labor.name = "Labor";
            labor.transform.parent = laborGroup.transform;
            labor.SetActive(true);
            labor.GetComponent<ProductLabor>().Fire();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update() {
        // TO DO The possibility of this should scale with difficulty.

        // Randomly spawn shells.
        if (!shell.active && Random.value < 0.001) {
            if (Random.value < 0.5) {
                shell.type = primaryType;
            } else {
                shell.type = ProductLabor.RandomType;
            }
            shell.gameObject.SetActive(true);
        }
    }

}
