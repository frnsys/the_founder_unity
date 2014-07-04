using UnityEngine;
using System.Collections;

public class UIWorker : MonoBehaviour {
    public Worker worker;

    public void HireWorker() {
        Debug.Log(worker.name);
        GameManager.Instance.HireWorker(worker);
    }
}


