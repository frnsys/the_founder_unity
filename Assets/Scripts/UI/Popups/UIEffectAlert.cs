using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// A text alert popup which supports rendering effects.
public class UIEffectAlert : UIAlert {
    public UIGrid effectGrid;
    private List<UIWidget> effectWidgets = new List<UIWidget>();
    protected int height;

    public GameObject effectPrefab;
    public GameObject buffEffectPrefab;
    public GameObject productEffectPrefab;

    void OnEnable() {
        height = gameObject.GetComponent<UIWidget>().height;
    }

    public void RenderEffects(EffectSet es) {
        // Clear out existing effect elements.
        for (int i = effectGrid.transform.childCount - 1; i >= 0; i--) {
            GameObject go = effectGrid.transform.GetChild(i).gameObject;
            NGUITools.DestroyImmediate(go);
        }

        RenderUnlockEffects(es);
        RenderBuffEffects(es);
        RenderProductEffects(es);
        RenderSpecialEffects(es);
        effectGrid.Reposition();
    }

    public void AdjustEffectsHeight() {
        int count = effectGrid.GetChildList().Count;

        // If there are effects, expand the height for them.
        if (count > 0)
            Extend((int)(count * effectGrid.cellHeight));
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
        foreach (Perk i in es.unlocks.perks) {
            RenderUnlockEffect(i.name);
        }
        foreach (Location i in es.unlocks.locations) {
            RenderUnlockEffect(i.name);
        }
        foreach (MiniCompany i in es.unlocks.companies) {
            RenderUnlockEffect(i.name);
        }
        foreach (SpecialProject i in es.unlocks.specialProjects) {
            RenderUnlockEffect(i.name);
        }
    }

    private void RenderBuffEffects(EffectSet es) {
        foreach (StatBuff buff in es.workerEffects) {
            if (buff.value != 0)
                RenderBuffEffect(buff, "workers");
        }

        if (es.research.value != 0) {
            RenderBuffEffect(es.research, null);
        }

        if (es.cash != 0) {
            RenderBuffEffect(new StatBuff("Cash", es.cash), null);
        }

        if (es.forgettingRate != 0) {
            RenderEffect(string.Format("Consumers forget bad publicity {0:P1} {1}",
                            Mathf.Abs(es.forgettingRate),
                            es.forgettingRate > 0 ? "faster" : "slower"));
        }

        if (es.spendingMultiplier != 0) {
            RenderEffect(string.Format("Consumers consume {0:P1} {1} stuff",
                            Mathf.Abs(es.spendingMultiplier),
                            es.spendingMultiplier > 0 ? "more" : "less"));
        }

        if (es.wageMultiplier != 0) {
            RenderEffect(string.Format("Wages {1} by {0:P1}",
                            Mathf.Abs(es.wageMultiplier),
                            es.wageMultiplier > 0 ? "increase" : "fall"));
        }

        if (es.economicStability != 0) {
            RenderEffect(string.Format("Economic stability {0}",
                            es.economicStability > 0 ? "increases" : "decreases"));
        }

        if (es.taxRate != 0) {
            RenderEffect(string.Format("Taxes {1} by {0:P1}",
                            Mathf.Abs(es.taxRate),
                            es.taxRate > 0 ? "increases" : "fall"));
        }

        if (es.expansionCostMultiplier != 0) {
            RenderBuffEffect(new StatBuff("Expansion Costs", es.expansionCostMultiplier), null);
            RenderEffect(string.Format("New locations are {0:P1} {1}",
                            Mathf.Abs(es.expansionCostMultiplier),
                            es.expansionCostMultiplier > 0 ? "more expensive" : "cheaper"));
        }

        if (es.opinionEvent.opinion.value != 0) {
            RenderBuffEffect(es.opinionEvent.opinion, null);
        }

        if (es.opinionEvent.publicity.value != 0) {
            RenderBuffEffect(es.opinionEvent.publicity, null);
        }

        foreach (Infrastructure.Type t in Infrastructure.Types) {
            if (es.infrastructureCostMultiplier[t] != 0) {
                RenderBuffEffect(new StatBuff(string.Format("{0} costs", t), es.infrastructureCostMultiplier[t]/100f), null);
            }
        }
    }

    private void RenderProductEffects(EffectSet es) {
        foreach (ProductEffect pe in es.productEffects) {
            if (pe.buff.value != 0) {
                GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, productEffectPrefab);
                effectObj.GetComponent<UIProductEffect>().Set(pe);
                effectWidgets.Add(effectObj.GetComponent<UIWidget>());
            }
        }
    }

    private void RenderSpecialEffects(EffectSet es) {
        if (es.specialEffect != EffectSet.Special.None) {
            RenderSpecialEffect(es.specialEffect);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, effectPrefab);
        effectObj.GetComponent<UIEffect>().SetUnlock(name);
        effectWidgets.Add(effectObj.GetComponent<UIWidget>());
    }

    private void RenderSpecialEffect(EffectSet.Special effect) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, effectPrefab);
        effectObj.GetComponent<UIEffect>().SetSpecial(effect);
        effectWidgets.Add(effectObj.GetComponent<UIWidget>());
    }

    private void RenderBuffEffect(StatBuff buff, string target) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, buffEffectPrefab);
        effectObj.GetComponent<UIBuffEffect>().Set(buff, target);
        effectWidgets.Add(effectObj.GetComponent<UIWidget>());
    }

    private void RenderEffect(string desc) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, effectPrefab);
        effectObj.GetComponent<UIEffect>().Set(desc);
        effectWidgets.Add(effectObj.GetComponent<UIWidget>());
    }

    protected override void Extend(int amount) {
        amount = (amount/2) + 8;
        int currentBottom = body.bottomAnchor.absolute;
        int currentTop = body.topAnchor.absolute;
        body.bottomAnchor.Set(window.transform, 0, currentBottom-amount);
        body.topAnchor.Set(window.transform, 0, currentTop+amount);
    }

    // Call this in the update loop to keep effects at full width.
    protected void UpdateEffectWidths() {
        int w = effectGrid.GetComponent<UIWidget>().width;
        foreach (UIWidget widget in effectWidgets) {
            widget.width = w;
        }
    }
}
