using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageSpecialProjects : UIFullScreenPager {
    private GameManager gm;
    private Company company;
    public GameObject specialProjectPrefab;
    public GameObject blackout;
    public UIProgressBar progressBar;

    void OnEnable() {
        gm = GameManager.Instance;
        company = gm.playerCompany;
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

    void Update() {
        // Prevent access if something is already developing.
        if (company.developing) {
            blackout.SetActive(true);

            if (company.developingSpecialProject != null) {
                progressBar.value = company.developingSpecialProject.progress;
            } else {
                progressBar.gameObject.SetActive(false);
            }

        } else {
            blackout.SetActive(false);
        }
    }
}


