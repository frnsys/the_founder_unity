using UnityEngine;
using System;
using System.Collections;

public class UIResearch : UIWindow {
    private GameManager gm;
    public GameObject consultancyItemPrefab;
    public GameObject consultancyList;
    public GameObject consultancyView;

    public void ShowConsultancies() {
        consultancyList.SetActive(true);
        consultancyView.SetActive(false);
    }
    public void HideConsultancies() {
        consultancyList.SetActive(false);
        consultancyView.SetActive(true);
    }

    protected override void PreSetup(GameObject obj) {
        gm = GameManager.Instance;
        UpdateCurrent();
    }

    protected override void Setup(GameObject obj) {
        // Load the unlocked consultancies.
        GameObject grid = transform.Find("Consultancies/Content Scroll/Items Grid").gameObject;
        foreach (Consultancy c in gm.unlocked.consultancies) {
            GameObject item = NGUITools.AddChild(grid, consultancyItemPrefab);
            item.GetComponent<UIConsultancyItem>().consultancy = c;

            GameObject hireButton = item.transform.Find("Hire").gameObject;
            UIEventListener.Get(hireButton).onClick += Hire;

            if (gm.researchManager.consultancy == c) {
                SetCurrentHireButton(hireButton);
            }
        }
        grid.GetComponent<UICenteredGrid>().Reposition();
    }

    private void UpdateCurrent() {
        // Set the current discovery being researched.
        UILabel discovery = transform.Find("Current/Researching/Discovery Name").GetComponent<UILabel>();
        if (gm.researchManager.discovery != null) {
            Discovery currentDiscovery = gm.researchManager.discovery;
            discovery.text = currentDiscovery.name;
        } else {
            discovery.text = "none";
        }

        // Set the current consultancy.
        UILabel consultancy = transform.Find("Current/Consultancy/Consultancy Name").GetComponent<UILabel>();
        if (gm.researchManager.consultancy != null) {
            Consultancy currentConsultancy = gm.researchManager.consultancy;
            consultancy.text = currentConsultancy.name;
        } else {
            consultancy.text = "none";
        }

        // TO DO show progress of current discovery.
        Research currentResearch = gm.researchManager.research;
    }

    public void Hire(GameObject gameObj) {
        // Not very elegant...
        // Go up and get the UIConsultancyItem this button belongs to.
        Consultancy consultancy = gameObj.transform.parent.gameObject.GetComponent<UIConsultancyItem>().consultancy;

        Action yes = delegate() {
            // Hire the consultancy...
            gm.researchManager.HireConsultancy(consultancy);
            UpdateCurrent();

            // Disable this button.
            SetCurrentHireButton(gameObj);

        };
        if (gm.researchManager.consultancy) {
            UIManager.Instance.Confirm("This will replace " + gm.researchManager.consultancy.name + ", are you sure?", yes, null);
        } else {
            yes();
        }
    }

    private GameObject currentHiredButton;
    private void SetCurrentHireButton(GameObject go) {
        go.GetComponent<UIButton>().isEnabled = false;
        go.transform.Find("Hire Label").GetComponent<UILabel>().text = "Hired";

        if (currentHiredButton != null) {
            // Renable the previous button.
            currentHiredButton.GetComponent<UIButton>().isEnabled = true;
            currentHiredButton.transform.Find("Hire Label").GetComponent<UILabel>().text = "Hire";
            currentHiredButton = go;
        }
    }
}
