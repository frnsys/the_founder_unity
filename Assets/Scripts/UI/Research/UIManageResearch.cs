using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIManageResearch : UIFullScreenPager {
    private ResearchManager rm;

    public GameObject techPrefab;
    public UILabel techLabel;
    public UITexture techIcon;
    public UIProgressBar progress;

    void OnEnable() {
        rm = GameManager.Instance.researchManager;
        LoadTechs();
    }

    private void LoadTechs() {
        ClearGrid();
        foreach (Technology t in rm.AvailableTechnologies().Where(t => t != rm.technology)) {
            GameObject techItem = NGUITools.AddChild(grid.gameObject, techPrefab);
            techItem.GetComponent<UITechnology>().technology = t;
        }
        Adjust();
    }

    void Update() {
        if (rm.technology == null) {
            techLabel.text = "No current research.";
            techIcon.mainTexture = null;
        } else {
            techLabel.text = rm.technology.name;
            techIcon.mainTexture = rm.technology.icon;
        }
        progress.value = rm.progress;
    }
}
