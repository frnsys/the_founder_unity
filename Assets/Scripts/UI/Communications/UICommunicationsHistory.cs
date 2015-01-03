using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UICommunicationsHistory : MonoBehaviour {
    public GameObject eventItemPrefab;
    public UICenteredGrid grid;
    private Company playerCompany;
    private List<OpinionEvent> displayedEvents = new List<OpinionEvent>();

    void OnEnable() {
        playerCompany = GameManager.Instance.playerCompany;
        foreach (OpinionEvent oe in playerCompany.OpinionEvents) {
            if (!displayedEvents.Contains(oe)) {
                GameObject eventItem = NGUITools.AddChild(grid.gameObject, eventItemPrefab);
                eventItem.transform.Find("Name").GetComponent<UILabel>().text = oe.name;
                eventItem.transform.Find("Opinion Effect").GetComponent<UILabel>().text = oe.opinion.value > 0 ? oe.opinion.value.ToString() : "forgotten";
                displayedEvents.Add(oe);
            } else {
                int childIdx = displayedEvents.IndexOf(oe);
                grid.transform.GetChild(childIdx).Find("Opinion Effect").GetComponent<UILabel>().text = oe.opinion.value > 0 ? oe.opinion.value.ToString() : "forgotten";
            }
        }
        grid.Reposition();
    }
}
