using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpecialProject : UIEffectItem {
    private SpecialProject _specialProject;
    public SpecialProject specialProject {
        get { return _specialProject; }
        set {
            _specialProject = value.Clone();

            // Check if the project has already been completed.
            SpecialProject completedProject = SpecialProject.Find(_specialProject, company.specialProjects);
            if (completedProject == null) {
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
        buttonLabel.text = "Begin Development";

        if (!_specialProject.isAvailable(company)) {
            button.isEnabled = false;
        }
    }

    void SetupCompletedProject() {
        costLabel.gameObject.SetActive(false);
        button.isEnabled = false;
        buttonLabel.text = "Completed";
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        UIEventListener.Get(button.gameObject).onClick += BeginProject;
    }

    void Update() {
        // Rotate the product, fancy.
        projectObj.transform.Rotate(0,0,-50*Time.deltaTime);
        UpdateEffectWidths();

        int w = requiredProductsGrid.GetComponent<UIWidget>().width;
        foreach (UIWidget widget in prereqWidgets) {
            widget.width = w;
        }
    }

    public void BeginProject(GameObject obj) {
        UIManager.Instance.Confirm("This project will cost " + costLabel.text + " to develop.", delegate() {
            if (!company.StartSpecialProject(_specialProject)) {
                UIManager.Instance.Alert("You can't afford the cost for this project.");
            }
        }, null);
    }
}
