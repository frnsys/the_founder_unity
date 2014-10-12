using UnityEngine;
using System.Collections;

public class UIMarketItem : MonoBehaviour {
    private Item item_;
    public Item item {
        get { return item_; }
        set {
            item_ = value;
            transform.Find("Name").GetComponent<UILabel>().text = item_.name;
            transform.Find("Price").GetComponent<UILabel>().text = "$" + item_.cost.ToString();
        }
    }
}
