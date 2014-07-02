using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    private GameManager gm;

    public UILabel cashLabel;
    public UILabel timeLabel;
    public UILabel workersLabel;

    void Start() {
        gm = GameManager.Instance;
    }

    void Update() {
        cashLabel.text = "$" + gm.playerCompany.cash;
        timeLabel.text = "Y" + gm.year + ": " + gm.month;

        workersLabel.text = "Workers: " + gm.playerCompany.workers.Count;
    }

    public void HireWorker() {
        gm.HireWorker();
    }
}


