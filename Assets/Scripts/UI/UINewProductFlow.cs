using UnityEngine;
using System.Collections;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    void OnEnable() {
        gm = GameManager.Instance;
    }

    void Update() {
    }


    public void SelectProductType() {
        Debug.Log("Clicked");
    }
}


