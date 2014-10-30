/*
 * A text alert popup which supports rendering effects.
 */

using UnityEngine;
using System.Collections;

public class UIEffectAlert : UIAlert {
    public UIGrid effectGrid;

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

        // -1 because by default there is space for about 1 effect.
        Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
    }

    private void RenderUnlockEffects(EffectSet es) {
        // Render the unlock effects for this event.
        // Note that event unlocks are *not* rendered because
        // those are "hidden" effects.
        foreach (Industry i in es.unlocks.industries) {
            RenderUnlockEffect("the " + i.name + " industry");
        }
        foreach (ProductType i in es.unlocks.productTypes) {
            RenderUnlockEffect(i.name + " products");
        }
        foreach (Market i in es.unlocks.markets) {
            RenderUnlockEffect("the " + i.name + " market");
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
            RenderBuffEffect(buff.name);
        }
        foreach (StatBuff buff in es.company) {
            RenderBuffEffect(buff.name);
        }
    }

    private void RenderProductEffects(EffectSet es) {
        foreach (ProductEffect pe in es.products) {
            GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, productEffectPrefab);
            effectObj.GetComponent<UIUnlockEffect>().Set(name);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, unlockEffectPrefab);
        effectObj.GetComponent<UIUnlockEffect>().Set(name);
    }

    private void RenderBuffEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, buffEffectPrefab);
        effectObj.GetComponent<UIUnlockEffect>().Set(name);
    }
}
