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
using System.Collections;

public class UIProduct : MonoBehaviour {
    private Product product_;
    public Product product {
        get { return product_; }
        set {
            product_ = value;

            // This never changes so just set it once.
            infrastructure.text = product_.requiredInfrastructure.ToString();

            SetData();
        }
    }
    private Product.State state;

    void Update() {
        if (product_ != null)
            SetData();
    }

    private void SetData() {
        // These things only need to update if product the state has changed.
        if (state != product_.state) {
            if (product_.developing) {
                name.text = product_.genericName;
                status.text = "Developing";
                description.gameObject.SetActive(false);
            }
            else {
                name.text = product_.name;
                progress.gameObject.SetActive(false);
                description.gameObject.SetActive(true);
                RenderEffects(product_.effects);
                AdjustEffectsHeight();
            }

            if (product_.retired) {
                shutdown.isEnabled = false;
                shutdown.transform.Find("Label").GetComponent<UILabel>().text = "Discontinued";
            }
            state = product_.state;
        }

        // These things are always updating.
        if (product_.developing) {
            progress.value = product_.progress;
        } else {
            status.text = "$" + ((int)product_.revenueEarned).ToString();
        }
    }

    public UILabel name;
    public UILabel status;
    public UILabel infrastructure;
    public UILabel description;
    public UIButton shutdown;
    public UIProgressBar progress;
    public UIGrid effectGrid;

    public GameObject buffEffectPrefab;
    public GameObject unlockEffectPrefab;
    public GameObject productEffectPrefab;
    private void RenderEffects(EffectSet es) {
        // Clear out existing effect elements.
        while (effectGrid.transform.childCount > 0) {
            GameObject go = effectGrid.transform.GetChild(0).gameObject;
            NGUITools.DestroyImmediate(go);
        }

        RenderUnlockEffects(es);
        RenderBuffEffects(es);
        RenderProductEffects(es);
    }

    public void AdjustEffectsHeight() {
        int count = effectGrid.GetChildList().Count;

        // If there are effects, expand the height for them.
        if (count > 0)
            Extend((int)((count + 1) * effectGrid.cellHeight));
    }

    private void RenderUnlockEffects(EffectSet es) {
        // Render the unlock effects for this event.
        // Note that event unlocks are *not* rendered because
        // those are "hidden" effects. You don't know they can happen until they do happen.
        foreach (ProductType i in es.unlocks.productTypes) {
            RenderUnlockEffect(i.name + " products");
        }
        foreach (Vertical i in es.unlocks.verticals) {
            RenderUnlockEffect("the " + i.name + " vertical");
        }
        foreach (Worker i in es.unlocks.workers) {
            RenderUnlockEffect(i.name);
        }
        foreach (Item i in es.unlocks.items) {
            RenderUnlockEffect(i.name);
        }
    }

    private void RenderBuffEffects(EffectSet es) {
        foreach (StatBuff buff in es.workers) {
            RenderBuffEffect(buff, "workers");
        }
        foreach (StatBuff buff in es.company) {
            RenderBuffEffect(buff, "the company");
        }
    }

    private void RenderProductEffects(EffectSet es) {
        foreach (ProductEffect pe in es.products) {
            GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, productEffectPrefab);
            effectObj.GetComponent<UIProductEffect>().Set(pe);
            effectObj.GetComponent<UIWidget>().leftAnchor.Set(description.transform, 0, 0);
            effectObj.GetComponent<UIWidget>().rightAnchor.Set(description.transform, 0, 0);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, unlockEffectPrefab);
        effectObj.GetComponent<UIUnlockEffect>().Set(name);
        effectObj.GetComponent<UIWidget>().leftAnchor.Set(description.transform, 0, 0);
        effectObj.GetComponent<UIWidget>().rightAnchor.Set(description.transform, 0, 0);
    }

    private void RenderBuffEffect(StatBuff buff, string target) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, buffEffectPrefab);
        effectObj.GetComponent<UIBuffEffect>().Set(buff, target);
        Debug.Log("rendering");
        Debug.Log(description.width);
        Debug.Log(description.localSize.x);
        effectObj.GetComponent<UIWidget>().leftAnchor.Set(description.transform, 0, 0);
        effectObj.GetComponent<UIWidget>().rightAnchor.Set(description.transform, 0, 0);
    }

    public void Extend(int amount) {
        gameObject.GetComponent<UIWidget>().height += amount;
    }

}


