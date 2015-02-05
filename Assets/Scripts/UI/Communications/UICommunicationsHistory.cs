using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UICommunicationsHistory : MonoBehaviour {
    public GameObject eventItemPrefab;
    public UICenteredGrid grid;
    private Company playerCompany;
    private List<OpinionEvent> displayedEvents = new List<OpinionEvent>();
    private List<UILabel> displayedLabels = new List<UILabel>();

    void OnEnable() {
        playerCompany = GameManager.Instance.playerCompany;
        foreach (OpinionEvent oe in playerCompany.OpinionEvents) {
            if (!displayedEvents.Contains(oe)) {
                GameObject eventItem = NGUITools.AddChild(grid.gameObject, eventItemPrefab);
                eventItem.transform.Find("Name").GetComponent<UILabel>().text = oe.name;

                UILabel effect = eventItem.transform.Find("Opinion Effect").GetComponent<UILabel>();
                effect.text = oe.opinion.value > 0 ? oe.opinion.value.ToString() : "forgotten";
                displayedEvents.Add(oe);
                displayedLabels.Add(effect);
            } else {
                int childIdx = displayedEvents.IndexOf(oe);
                displayedLabels[childIdx].text = oe.opinion.value > 0 ? oe.opinion.value.ToString() : "forgotten";
            }
        }
        grid.Reposition();
    }
}
