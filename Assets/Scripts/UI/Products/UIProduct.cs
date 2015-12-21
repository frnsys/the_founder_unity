using UnityEngine;
using System.Linq;
using System.Collections;

public class UIProduct : MonoBehaviour {
    private ProductRecipe recipe_;
    public ProductRecipe recipe {
        get { return recipe_; }
        set {
            recipe_ = value;
            if (GameManager.Instance.playerCompany.discoveredProducts.Contains(recipe_)) {
                label.text = recipe_.name;
                discovered = true;

                if (recipe_.productTypes.Count() >= 2) {
                    productObject1.GetComponent<MeshFilter>().mesh = recipe_.productTypes[0].mesh;
                    productObject2.GetComponent<MeshFilter>().mesh = recipe_.productTypes[1].mesh;
                }
            } else {
                label.text = "???";
                discovered = false;
                Lock();

                productObject1.SetActive(false);
                productObject2.SetActive(false);
            }

        }
    }

    void Update() {
        UIAnimator.Rotate(productObject1);
        UIAnimator.Rotate(productObject2);
    }

    private bool discovered;
    public UILabel label;
    public GameObject productObject1;
    public GameObject productObject2;

    void OnClick() {
        if (discovered) {
            UIManager.Instance.Alert(recipe_.description);
        }
    }

    public void Lock() {
        transform.Find("Overlay").gameObject.SetActive(true);
    }
}


