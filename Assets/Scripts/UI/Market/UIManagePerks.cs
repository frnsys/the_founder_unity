using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManagePerks : MonoBehaviour {
    private GameManager gm;

    public GameObject perkPrefab;
    public UISimpleGrid grid;

    public List<Perk> displayedPerks = new List<Perk>();

    void OnEnable() {
        gm = GameManager.Instance;
    }

    void Update() {
        foreach (Perk perk in gm.playerCompany.perks) {
            if (!displayedPerks.Contains(perk)) {
                GameObject perkObj = NGUITools.AddChild(grid.gameObject, perkPrefab);
                perkObj.GetComponent<UIPerk>().perk = perk;
                displayedPerks.Add(perk);
            }
        }
        grid.Reposition();
    }
}


