using UnityEngine;
using System.Collections;

public class UIResearch : UIWindow {
    public GameObject consultancyItemPrefab;
    public GameObject consultancyList;
    public GameObject consultancyView;

    public void Hire(GameObject gameObj) {
        // Not very elegant...
        // Go up and get the UIConsultancyItem this button belongs to.
        UIConsultancyItem consultancyItem = gameObj.transform.parent.gameObject.GetComponent<UIConsultancyItem>();

        // Hire the consultancy...
        GameManager.Instance.HireConsultancy(consultancyItem.consultancy);

        SetCurrent();
    }

    public void ShowConsultancies() {
        consultancyList.SetActive(true);
        consultancyView.SetActive(false);
    }
    public void HideConsultancies() {
        consultancyList.SetActive(false);
        consultancyView.SetActive(true);
    }

    void OnEnable() {
        // Load the unlocked consultancies.
        GameObject grid = transform.Find("Consultancies/Content Scroll/Items Grid").gameObject;
        foreach (Consultancy c in GameManager.Instance.unlocked.consultancies) {
            GameObject item = NGUITools.AddChild(grid, consultancyItemPrefab);
            item.GetComponent<UIConsultancyItem>().consultancy = c;
            UIEventListener.Get(item.transform.Find("Hire").gameObject).onClick += Hire;
        }
        grid.GetComponent<UICenteredGrid>().Reposition();

        SetCurrent();
    }

    private void SetCurrent() {
        // Set the current research and consultancy.
        if (GameManager.Instance.researchManager.discovery != null) {
            Discovery currentDiscovery = GameManager.Instance.researchManager.discovery;
            transform.Find("Current/Researching/Discovery Name").GetComponent<UILabel>().text = currentDiscovery.name;
        } else {
            transform.Find("Current/Researching/Discovery Name").GetComponent<UILabel>().text = "none";
        }
        if (GameManager.Instance.researchManager.consultancy != null) {
            Consultancy currentConsultancy = GameManager.Instance.researchManager.consultancy;
            transform.Find("Current/Consultancy/Consultancy Name").GetComponent<UILabel>().text = currentConsultancy.name;
        } else {
            transform.Find("Current/Consultancy/Consultancy Name").GetComponent<UILabel>().text = "none";
        }
        Research currentResearch = GameManager.Instance.researchManager.research;
    }
}
