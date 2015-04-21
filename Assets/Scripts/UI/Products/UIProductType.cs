using UnityEngine;
using System.Linq;
using System.Collections;

public class UIProductType : MonoBehaviour {
    private ProductType productType_;
    public ProductType productType {
        get { return productType_; }
        set {
            productType_ = value;
            if (productType_.isAvailable(GameManager.Instance.playerCompany)) {
                label.text = productType_.name;
            } else {
                label.text = "???";
            }
            productObject.GetComponent<MeshFilter>().mesh = productType_.mesh;
        }
    }

    void Update() {
        UIAnimator.Rotate(productObject);
    }

    public UILabel label;
    public GameObject productObject;

    void OnClick() {
        if (!productType_.isAvailable(GameManager.Instance.playerCompany)) {
            string requirements = "";

            // Find required technology, if there is one.
            Technology requiredTech = Technology.LoadAll().FirstOrDefault(t => t.effects.unlocks.productTypes.Contains(productType_));

            if (productType_.requiredVerticals.Count > 1) {
                requirements = string.Format("Requires the {0} verticals", string.Join(", ", productType_.requiredVerticals.Select(v => v.name).ToArray()));
            } else {
                requirements = string.Format("Requires the {0} vertical", productType_.requiredVerticals[0].name);
            }

            if (requiredTech == null) {
                requirements += ".";
            } else {
                requirements += string.Format(" and the {0} technology.", requiredTech.name);
            }

            UIManager.Instance.Alert(requirements);
        }
    }
}


