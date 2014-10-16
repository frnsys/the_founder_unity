using UnityEngine;
using System.Collections;

public class UIResearch : UIWindow {
    public GameObject inProgressOverlay;
    public UIProgressBar progressBar;
    public GameObject consultancyItemPrefab;
    public bool researchComplete;

    public void Hire(GameObject gameObj) {
        // Not very elegant...
        // Go up and get the UIConsultancyItem this button belongs to.
        UIConsultancyItem consultancyItem = gameObj.transform.parent.gameObject.GetComponent<UIConsultancyItem>();

        // If we successfully hire the consultancy...
        if (GameManager.Instance.playerCompany.Research(consultancyItem.consultancy)) {
            // Blackout the research screen,
            // Show the research progress bar.
            inProgressOverlay.SetActive(true);
            progressBar.value = 0;

            // Show the research progress bar on the main screen.
        }
    }

    void OnEnable() {
        if (GameManager.Instance.playerCompany.researching) {
            inProgressOverlay.SetActive(true);
        }

        // Load the unlocked consultancies.
        GameObject grid = transform.Find("Content Scroll/Items Grid").gameObject;
        foreach (Consultancy c in GameManager.Instance.unlocked.consultancies) {
            GameObject item = NGUITools.AddChild(grid, consultancyItemPrefab);
            item.GetComponent<UIConsultancyItem>().consultancy = c;
            UIEventListener.Get(item.transform.Find("Hire").gameObject).onClick += Hire;
        }
        grid.GetComponent<UICenteredGrid>().Reposition();
    }

    void Update() {
        if (GameManager.Instance.playerCompany.researching) {
            progressBar.value = GameManager.Instance.playerCompany.consultancy.researchProgress;
        } else {
            inProgressOverlay.SetActive(false);
        }
    }
}
