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
            UpdateTotal();

            RenderEffects(item.effects);
            AdjustEffectsHeight();
        }
    }

    private int quantity_ = 1;
    private int quantity {
        get { return quantity_; }
        set {
            quantity_ = value;
            quantityLabel.text = quantity.ToString();
            UpdateTotal();
        }
    }

    // UI
    public UILabel quantityLabel;
    public UILabel totalLabel;
    public UILabel nameLabel;
    public UILabel descLabel;

    public void AddItem() {
        if (quantity < 99) {
            quantity++;
        }
    }
    public void SubtractItem() {
        if (quantity > 1) {
            quantity--;
        }
    }
    public void UpdateTotal() {
        if (item) {
            totalLabel.text = string.Format("${0:n}", quantity * item.cost);
        } else {
            totalLabel.text = string.Format("${0:n}", 0);
        }
    }

    public void Buy() {
        for (int i=0; i<quantity; i++) {
            GameManager.Instance.playerCompany.BuyItem(item);
        }
    }
}
