using UnityEngine;
using System.Linq;
using System.Collections;

public class UIEffectItem : MonoBehaviour {
    public UIGrid effectGrid;
    public GameObject anchorTarget;

    public GameObject buffEffectPrefab;
    public GameObject unlockEffectPrefab;
    public GameObject productEffectPrefab;
    public void RenderEffects(EffectSet es) {
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

    private void RenderBuffEffects(EffectSet es) {
        foreach (StatBuff buff in es.workerEffects) {
            RenderBuffEffect(buff, "workers");
        }
        // TO DO
        //foreach (StatBuff buff in es.ofType<CashEffect>().Select(c => c.cash)) {
            //RenderBuffEffect(buff, "the company");
        //}
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
        foreach (Promo i in es.unlocks.promos) {
            RenderUnlockEffect(i.name);
        }
        foreach (Perk i in es.unlocks.perks) {
            RenderUnlockEffect(i.name);
        }
        foreach (Recruitment i in es.unlocks.recruitments) {
            RenderUnlockEffect(i.name);
        }
        foreach (Location i in es.unlocks.locations) {
            RenderUnlockEffect(i.name);
        }
    }

    private void RenderProductEffects(EffectSet es) {
        foreach (ProductEffect pe in es.productEffects) {
            GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, productEffectPrefab);
            effectObj.GetComponent<UIProductEffect>().Set(pe);
            effectObj.GetComponent<UIWidget>().leftAnchor.Set(anchorTarget.transform, 0, 0);
            effectObj.GetComponent<UIWidget>().rightAnchor.Set(anchorTarget.transform, 0, 0);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, unlockEffectPrefab);
        effectObj.GetComponent<UIUnlockEffect>().Set(name);
    }

    private void RenderBuffEffect(StatBuff buff, string target) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, buffEffectPrefab);
        effectObj.GetComponent<UIBuffEffect>().Set(buff, target);
        effectObj.GetComponent<UIWidget>().leftAnchor.Set(anchorTarget.transform, 0, 0);
        effectObj.GetComponent<UIWidget>().rightAnchor.Set(anchorTarget.transform, 0, 0);
    }

    public void Extend(int amount) {
        gameObject.GetComponent<UIWidget>().height += amount;
    }

}


