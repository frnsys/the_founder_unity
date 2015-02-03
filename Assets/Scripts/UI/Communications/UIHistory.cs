using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIHistory : MonoBehaviour {
    private GameManager gm;
    private Company company;

    public GameObject historyPrefab;
    public UIGrid grid;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
        LoadHistory();
    }

    private void LoadHistory() {
        while (grid.transform.childCount > 0)
            NGUITools.Destroy(grid.transform.GetChild(0).gameObject);

        foreach (OpinionEvent oe in company.OpinionEvents) {
            GameObject historyItem = NGUITools.AddChild(grid.gameObject, historyPrefab);
            historyItem.GetComponent<UIOpinionEvent>().opinionEvent = oe;
        }

        grid.Reposition();
    }
}
