using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearchCompletedAlert: UIEffectAlert {
    public UILabel nameLabel;
    public GameObject technologyPrefab;
    public GameObject selectedTechnologyItem;

    // For mapping GameObjects (by id) to technologies.
    public Dictionary<int, Technology> technologies = new Dictionary<int, Technology>();

    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            nameLabel.text = technology_.name;
            bodyLabel.text = technology_.description;
            Extend(bodyLabel.height);

            RenderEffects(technology.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    public void ShowNewResearchOptions() {
        transform.Find("Notification Window").gameObject.SetActive(false);
        transform.Find("New Research Window").gameObject.SetActive(true);
    }

    void OnEnable() {
        Show();

        // TO DO needs to load the next three technologies to choose from.
        //foreach (Technology d in GameManager.Instance.researchManager.nextTechnologies) {
        //}
        // Temporary:
        Technology di = Resources.Load("Technologies/Electricity") as Technology;

        GameObject technologiesGrid = transform.Find("New Research Window/Body/Technologies").gameObject;
        int width = technologiesGrid.GetComponent<UIWidget>().width;

        List<Technology> nextTechnologies = new List<Technology>();
        nextTechnologies.Add(di);
        nextTechnologies.Add(di);
        nextTechnologies.Add(di);

        foreach (Technology d in nextTechnologies) {
            GameObject dItem = NGUITools.AddChild(technologiesGrid, technologyPrefab);
            dItem.transform.Find("Technology Name").GetComponent<UILabel>().text = d.name;

            // Have to manually set all of this.
            int height = dItem.transform.GetComponent<UITexture>().height;
            dItem.transform.GetComponent<UITexture>().width = width;
            dItem.transform.GetComponent<UITexture>().depth = 10;
            dItem.transform.GetComponent<BoxCollider>().size = new Vector3(width, height, 1);

            UIEventListener.Get(dItem).onClick += SelectTechnology;
            technologies.Add(dItem.GetInstanceID(), d);
        }
        SelectTechnology( technologiesGrid.transform.GetChild(0).gameObject );

        technologiesGrid.GetComponent<UICenteredGrid>().Reposition();
    }

    void SelectTechnology(GameObject obj) {
        // Revert previously selected item.
        if (selectedTechnologyItem != null) {
            selectedTechnologyItem.GetComponent<UIButton>().defaultColor = new Color(1f, 1f, 1f, 1f);
            selectedTechnologyItem.transform.Find("Technology Name").gameObject.GetComponent<UILabel>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        // Highlight selected item.
        selectedTechnologyItem = obj;
        obj.GetComponent<UIButton>().defaultColor = new Color(0.57f, 0.41f, 0.98f, 1f);
        obj.transform.Find("Technology Name").gameObject.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void ConfirmTechnology() {
        Technology d = technologies[selectedTechnologyItem.GetInstanceID()];
        GameManager.Instance.researchManager.BeginResearch(d);
        Close_();
    }
}


