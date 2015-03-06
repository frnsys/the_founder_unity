using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfrastructureItem : MonoBehaviour {
    public Infrastructure.Type type;

    public GameObject displayObject;
    public UILabel costLabel;
    public UILabel amountLabel;
    public UIButton buyButton;
    public UIButton desButton;

    void Update() {
        // Rotate the product, fancy.
        displayObject.transform.Rotate(0,0,-50*Time.deltaTime);
    }
}


