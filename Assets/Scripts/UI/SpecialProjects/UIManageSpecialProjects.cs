using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageSpecialProjects : UIFullScreenPager {
    private GameManager gm;
    public GameObject specialProjectPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadSpecialProjects();
    }

    private void LoadSpecialProjects() {
        ClearGrid();
        foreach (SpecialProject p in gm.unlocked.specialProjects) {
            GameObject projectItem = NGUITools.AddChild(grid.gameObject, specialProjectPrefab);
            projectItem.GetComponent<UISpecialProject>().specialProject = p;
        }
        Adjust();
    }
}


