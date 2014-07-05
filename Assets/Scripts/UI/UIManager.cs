using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    private GameManager gm;

    void OnEnable() {
        gm = GameManager.Instance;

        //GameObject newProductFlow = Resources.Load("UI/New Product Selection") as GameObject;
        //NGUITools.AddChild(gameObject, newProductFlow);


        //GameObject workersGrid = GameObject.Find("Available Workers/Grid");
        //foreach (Worker worker in gm.unlockedWorkers) {
            //GameObject availableWorker = Resources.Load("Prefabs/UI/Available Worker") as GameObject;
            //availableWorker.transform.FindChild("Worker Name").GetComponent<UILabel>().text = worker.name;
            //availableWorker.GetComponent<UIWorker>().worker = worker;
            //NGUITools.AddChild(workersGrid, availableWorker);
        //}
        //workersGrid.GetComponent<UIGrid>().Reposition();

    }

    void Update() {
    }

    public void TestLog() {
        Debug.Log("CLICKED");
    }
}


