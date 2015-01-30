using UnityEngine;
using System.Collections;

public class UIMarketItem : MonoBehaviour {
    private Item item_;
    public Item item {
        get { return item_; }
        set {
            item_ = value;
            nameLabel.text = item_.name;
            priceLabel.text = "$" + item_.cost.ToString();

            itemObj.GetComponent<MeshFilter>().mesh = item_.mesh;
            itemObj.GetComponent<MeshRenderer>().material.mainTexture = item_.texture;
        }
    }

    // Cheating a little, but make this also work with Perks.
    private Perk perk_;
    public Perk perk {
        get { return perk_; }
        set {
            perk_ = value;
            nameLabel.text = perk_.current.name;
            priceLabel.text = "$" + perk_.cost.ToString();

            itemObj.GetComponent<MeshFilter>().mesh = perk_.mesh;
            itemObj.GetComponent<MeshRenderer>().material.mainTexture = perk_.texture;
        }
    }

    public UILabel nameLabel;
    public UILabel priceLabel;
    public GameObject itemObj;

    void Update() {
        // Rotate the product, fancy.
        itemObj.transform.Rotate(0,0,-50*Time.deltaTime);
    }
}
