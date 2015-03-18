/*
 * Product Item
 * ===================
 *
 * A single product item,
 * to keep track of its progress
 * and revenue.
 *
 */

using UnityEngine;
using System.Linq;
using System.Collections;

public class UIProduct : UIEffectItem {
    private Product product_;
    public Product product {
        get { return product_; }
        set {
            product_ = value;

            // This never changes so just set it once.
            infrastructure.text = string.Format("Using {0}", product_.requiredInfrastructure);

            UpdateState();
            SetData();
        }
    }
    private Product.State state;

    void Update() {
        if (product_ != null) {
            // These things only need to update if product the state has changed.
            if (state != product_.state)
                UpdateState();
            SetData();

            // Rotate the product, fancy.
            for (int i=0; i<productObjects.Length; i++) {
                UIAnimator.Rotate(productObjects[i]);
            }
        }
    }

    private void UpdateState() {
        if (product_.developing) {
            name.text = product_.genericName;
            status.text = "Developing...";
            description.gameObject.SetActive(false);
            shutdown.transform.Find("Label").GetComponent<UILabel>().text = "Cancel";

            for (int i=0; i<product_.meshes.Length; i++) {
                productObjects[i].SetActive(false);
            }
        } else {
            name.text = product_.name;
            progress.gameObject.SetActive(false);
            description.gameObject.SetActive(true);
            shutdown.transform.Find("Label").GetComponent<UILabel>().text = "Discontinue";

            for (int i=0; i<product_.meshes.Length; i++) {
                productObjects[i].GetComponent<MeshFilter>().mesh = product_.meshes[i];
                productObjects[i].SetActive(true);
            }

            RenderEffects(product_.effects);
            AdjustEffectsHeight();
        }

        if (product_.retired) {
            shutdown.isEnabled = false;
            shutdown.transform.Find("Label").GetComponent<UILabel>().text = "Discontinued";
        }
        state = product_.state;
    }

    private void SetData() {
        // These things are always updating.
        if (product_.developing) {
            progress.value = product_.progress;
        } else {
            status.text = string.Format("{0:C0}", product_.revenueEarned);
        }
    }

    public UILabel name;
    public UILabel status;
    public UILabel infrastructure;
    public UILabel description;
    public UIButton shutdown;
    public UIProgressBar progress;
    public GameObject[] productObjects;
}


