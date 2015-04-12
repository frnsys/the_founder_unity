using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpecialProject : UIEffectItem {
    private SpecialProject _specialProject;
    public SpecialProject specialProject {
        get { return _specialProject; }
        set {
            _specialProject = value;

            // Check if the project has already been completed.
            if (company.specialProjects.Contains(_specialProject)) {
                SetupNewProject();
            } else {
                SetupCompletedProject();
            }

            DisplayProject();
        }
    }

    private Company company;

    public UILabel nameLabel;
    public UILabel descLabel;
    public UILabel costLabel;
    public UIButton button;
    public UILabel buttonLabel;
    public GameObject projectObj;

    public UIGrid requiredProductsGrid;
    public GameObject requiredProductPrefab;

    void DisplayProject() {
        nameLabel.text = _specialProject.name;
        descLabel.text = _specialProject.description;
        projectObj.GetComponent<MeshFilter>().mesh = _specialProject.mesh;

        RenderRequiredProducts();
        RenderEffects(_specialProject.effects);
        AdjustEffectsHeight();
    }

    private List<UIWidget> prereqWidgets = new List<UIWidget>();
    void RenderRequiredProducts() {
        foreach (ProductRecipe r in _specialProject.requiredProducts) {
            GameObject pObj = NGUITools.AddChild(requiredProductsGrid.gameObject, requiredProductPrefab);
            if (company.HasProduct(r)) {
                pObj.transform.Find("Label").GetComponent<UILabel>().text = string.Format("[c][56FB92]{0}[-][/c]", r.genericName);
            } else {
                pObj.transform.Find("Label").GetComponent<UILabel>().text = string.Format("{0}", r.genericName);
            }
            prereqWidgets.Add(pObj.GetComponent<UIWidget>());
        }
        requiredProductsGrid.Reposition();
    }

    void SetupNewProject() {
        costLabel.gameObject.SetActive(true);
        costLabel.text = string.Format("{0:C0}", _specialProject.cost);
        buttonLabel.text = "Buy";

        if (!_specialProject.isAvailable(company)) {
            button.isEnabled = false;
        }
    }

    void SetupCompletedProject() {
        costLabel.gameObject.SetActive(false);
        button.isEnabled = false;
        buttonLabel.text = "Owned";
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        UIEventListener.Get(button.gameObject).onClick += BeginProject;
    }

    void Update() {
        UIAnimator.Rotate(projectObj);
        UpdateEffectWidths();

        int w = requiredProductsGrid.GetComponent<UIWidget>().width;
        foreach (UIWidget widget in prereqWidgets) {
            widget.width = w;
        }
    }

    public void BeginProject(GameObject obj) {
        UIManager.Instance.Confirm("This project will cost " + costLabel.text + " to develop.", delegate() {
            if (!company.BuySpecialProject(_specialProject)) {
                UIManager.Instance.Alert("You can't afford the cost for this project.");
            }
        }, null);
    }
}
