using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    private GameManager gm;

    void OnEnable() {
        gm = GameManager.Instance;
    }

    void Update() {
    }

    public void TestLog() {
        Debug.Log("CLICKED");
    }

    public void ShowNewProductFlow() {
        GameObject newProductFlow = Resources.Load("UI/Products/New Product Selection") as GameObject;
        NGUITools.AddChild(gameObject, newProductFlow);
    }

    public void ShowHireWorker() {
        // to do
    }
}


