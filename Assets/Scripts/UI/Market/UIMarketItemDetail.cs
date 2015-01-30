using UnityEngine;
using System.Collections;

public class UIMarketItemDetail : UIEffectAlert {
    private Item item_;
    public Item item {
        get { return item_; }
        set {
            item_ = value;
            nameLabel.text = item.name;
            descLabel.text = item.description;
            totalLabel.text = string.Format("${0:n}", item.cost);

            RenderEffects(item.effects);
            AdjustEffectsHeight();
        }
    }

    // Cheating a little, but make this also work with Perks.
    private Perk perk_;
    public Perk perk {
        get { return perk_; }
        set {
            perk_ = value;
            nameLabel.text = perk.current.name;
            descLabel.text = perk.description;
            totalLabel.text = string.Format("${0:n}", perk.cost);

            itemObj.GetComponent<MeshFilter>().mesh = perk.mesh;
            itemObj.GetComponent<MeshRenderer>().material.mainTexture = perk.texture;

            RenderEffects(perk.effects);
            AdjustEffectsHeight();
        }
    }

    void Update() {
        // Rotate the product, fancy.
        float rotation = itemObj.transform.rotation.z;
        itemObj.transform.Rotate(0,0,rotation - 1.5f);
    }

    // UI
    public UILabel totalLabel;
    public UILabel nameLabel;
    public UILabel descLabel;
    public GameObject itemObj;

    public void Buy() {
        if (perk != null)
            GameManager.Instance.playerCompany.BuyPerk(perk);
        else
            GameManager.Instance.playerCompany.BuyItem(item);
        Close_();
    }
}
