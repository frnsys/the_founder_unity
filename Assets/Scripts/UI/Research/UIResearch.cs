using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIResearch : MonoBehaviour {
    public UISimpleGrid grid;
    public UILabel pointsLabel;
    public GameObject researcherPrefab;
    public GameObject subwindow;
    private Company company;

    public GameObject recruitingWindow;
    public void ShowRecruiting() {
        UIManager.Instance.CloseAndOpenPopup(recruitingWindow);
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        ShowResearchers();
    }

    public GameObject manageResearchPrefab;
    public void OpenManageResearch() {
        GameObject popup = NGUITools.AddChild(subwindow, manageResearchPrefab);
        popup.GetComponent<UIWidget>().SetAnchor(subwindow.gameObject, 0, 0, 0, 0);
    }

    private void ShowResearchers() {
        foreach (AWorker w in company.researchers) {
            GameObject reItem = NGUITools.AddChild(grid.gameObject, researcherPrefab);
            reItem.GetComponent<UIResearcher>().worker = w;
            SetupButton(reItem);
        }

        // Create empty spaces.
        for (int i=0; i < 12 - company.researchers.Count; i++) {
            GameObject reItem = NGUITools.AddChild(grid.gameObject, researcherPrefab);
            reItem.GetComponent<UIResearcher>().empty = true;
            SetupButton(reItem);
        }

        grid.Reposition();
    }

    private void SetupButton(GameObject button) {
        UIEventListener.VoidDelegate onTap = delegate(GameObject obj) {
            UIResearcher item = button.GetComponent<UIResearcher>();
            SetupPrompt();
            prompt.SetActive(true);
            currentItem = item;
        };
        UIEventListener.Get(button).onClick += onTap;
    }

    private UIResearcher currentItem;
    public GameObject prompt;
    public GameObject workerPrefab;
    public UIScrollView promptScrollView;
    public GameObject noWorkersNotice;

    public void AssignWorker(GameObject obj) {
        AWorker w = obj.GetComponent<UIResearcherItem>().worker;
        // If the this slot is currently occupied
        // remove the existing worker.
        if (!currentItem.empty) {
            company.RemoveResearcher(currentItem.worker);
        }

        company.AddResearcher(w);
        currentItem.worker = w;
        prompt.SetActive(false);
    }
    public void ClosePrompt() {
        currentItem = null;
        prompt.SetActive(false);
    }

    public UIGrid promptGrid;
    private void SetupPrompt() {
        // Clear out existing effect elements.
        for (int i = promptGrid.transform.childCount - 1; i >= 0; i--) {
            GameObject go = promptGrid.transform.GetChild(i).gameObject;
            NGUITools.DestroyImmediate(go);
        }

        IEnumerable<AWorker> workers = company.workers.Where(w => !company.researchers.Contains(w));

        if (workers.Count() == 0) {
            noWorkersNotice.SetActive(true);
        } else {
            noWorkersNotice.SetActive(false);
            foreach (AWorker w in workers) {
                GameObject reItem = NGUITools.AddChild(promptGrid.gameObject, workerPrefab);
                reItem.GetComponent<UIResearcherItem>().worker = w;
                reItem.GetComponent<UIDragScrollView>().scrollView = promptScrollView;
                UIEventListener.Get(reItem).onClick += AssignWorker;
            }
        }
        promptGrid.Reposition();
    }

    void Update() {
        pointsLabel.text = string.Format("{0} research", company.researchPoints);
    }
}


