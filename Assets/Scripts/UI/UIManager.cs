using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public UILabel cashLabel;
    public UILabel timeLabel;

    void Update() {
        cashLabel.text = "$" + GameManager.Instance.playerCompany.cash;
        timeLabel.text = "Y" + GameManager.Instance.year + ": " + GameManager.Instance.month;
    }
}


