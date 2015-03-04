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
            infrastructure.text = product_.requiredInfrastructure.ToString();

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
            productObject.transform.Rotate(0,0,-50*Time.deltaTime);
        }
    }

    private void UpdateState() {
        if (product_.developing) {
            name.text = product_.genericName;
            status.text = "Developing...";
            description.gameObject.SetActive(false);
            productObject.SetActive(false);
        } else {
            name.text = product_.name;
            progress.gameObject.SetActive(false);
            description.gameObject.SetActive(true);

            productObject.GetComponent<MeshFilter>().mesh = product.mesh;
            productObject.SetActive(true);

            RenderEffects(product_.effects);
            AdjustEffectsHeight();
        }

        if (product_.retired) {
            shutdown.isEnabled = false;
            shutdown.transform.Find("Label").GetComponent<UILabel>().text = "Discontinued";

            foreach (Transform t in effectGrid.transform) {
                t.gameObject.GetComponent<UIBuffEffect>().Disable();
            }
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
    public GameObject productObject;
}


